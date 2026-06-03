using Drajbot.Api.Data;
using Drajbot.Api.DTOs.Auth;
using Drajbot.Api.Enums;
using Drajbot.Api.Interfaces;
using Drajbot.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Drajbot.Api.Services.Auth
{
    public class AuthService(ApplicationDbContext context, IConfiguration config, IEmailService emailService) : IAuthService
    {
        public async Task<string> RegisterAsync(UserRegisterDto request)
        {
            if (await context.Users.AnyAsync(u => u.Email == request.Email)) return "Email je već u upotrebi.";
            if (await context.Users.AnyAsync(u => u.Username == request.Username)) return "Korisničko ime je zauzeto.";

            string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            var newUser = new User
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Username = request.Username,
                Email = request.Email,
                PasswordHash = passwordHash,
                Role = Role.User,
                LoyaltyRank = LoyaltyRank.None,
                AvatarUrl = $"https://ui-avatars.com/api/?name={request.Username}&background=808080&color=ffffff",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            context.Users.Add(newUser);
            await context.SaveChangesAsync();
            return "Uspesno";
        }

        public async Task<string> LoginAsync(UserLoginDto request)
        {
            // 1. Tražimo korisnika u bazi
            var user = await context.Users.FirstOrDefaultAsync(u =>
                    u.Email == request.UsernameOrEmail || u.Username == request.UsernameOrEmail);

            // 2. Proveravamo da li postoji i da li se lozinka poklapa (Koristimo BCrypt Verify)
            // Zbog bezbednosti, uvek vraćamo istu poruku da haker ne bi znao da li je pogrešio email ili šifru.
            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                return "Pogrešni kredencijali.";

            // 3. Generisanje JWT Tokena
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(config.GetSection("Jwt:Secret").Value!);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(
                [
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.Role.ToString()),
                    new Claim("LoyaltyRank", user.LoyaltyRank.ToString()) // Naš custom rank!
                ]),
                Expires = DateTime.UtcNow.AddDays(7), // Token traje 7 dana
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = config.GetSection("Jwt:Issuer").Value,
                Audience = config.GetSection("Jwt:Audience").Value
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public async Task<string> ForgotPasswordAsync(ForgotPasswordDto request)
        {
            var user = await context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (user == null)
                return "Ukoliko email postoji u bazi, poslat je kod za resetovanje."; // Ne odajemo hakeru da li mejl postoji!

            // 1. Generisanje random koda od 6 karaktera (slova i brojevi)
            string randomCode = Guid.NewGuid().ToString()[..6].ToUpper();

            // 2. Čuvanje u bazi (Ističe za 15 minuta)
            user.PasswordResetToken = randomCode;
            user.ResetTokenExpires = DateTime.UtcNow.AddMinutes(15);
            await context.SaveChangesAsync();

            // 3. Slanje emaila
            string emailBody = $@"
                <div style='font-family: Arial, sans-serif; background-color: #121212; color: #ffffff; padding: 20px; border-radius: 10px;'>
                    <h2 style='color: #00BCD4;'>D'RAJBOT Game Shop</h2>
                    <p>Primili smo zahtev za promenu lozinke.</p>
                    <p>Vaš kod za resetovanje je: <strong style='font-size: 24px; color: #00BCD4;'>{randomCode}</strong></p>
                    <p>Ovaj kod ističe za 15 minuta. Ako niste zatražili promenu, ignorišite ovu poruku.</p>
                </div>";

            // Namerno gađamo u Try-Catch, da nam server ne bi "pao" ako je loša Gmail šifra
            try
            {
                await emailService.SendEmailAsync(user.Email, "Resetovanje lozinke - D'RAJBOT", emailBody);
            }
            catch { return "Greška pri slanju emaila. Proverite konfiguraciju."; }

            return "Ukoliko email postoji u bazi, poslat je kod za resetovanje.";
        }

        public async Task<string> ResetPasswordAsync(ResetPasswordDto request)
        {
            var user = await context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (user == null) return "Greška: Neispravni podaci.";

            // 1. Provera da li je kod tačan i da li je istekao
            if (user.PasswordResetToken != request.Token)
                return "Greška: Neispravan kod za resetovanje.";

            if (user.ResetTokenExpires < DateTime.UtcNow)
                return "Greška: Kod je istekao. Zatražite novi.";

            // 2. Sve je u redu, heširamo novu šifru!
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);

            // 3. Poništavamo token da ne bi mogao ponovo da se iskoristi
            user.PasswordResetToken = null;
            user.ResetTokenExpires = null;

            await context.SaveChangesAsync();
            return "Lozinka je uspešno promenjena! Možete se prijaviti.";
        }
    }
}