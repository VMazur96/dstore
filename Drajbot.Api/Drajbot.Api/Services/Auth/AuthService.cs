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
    public class AuthService(ApplicationDbContext context, IConfiguration config) : IAuthService
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
    }
}