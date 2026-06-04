using Drajbot.Api.DTOs.Auth;
using Drajbot.Api.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Security.Claims;

namespace Drajbot.Api.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController(IAuthService authService, IAuditService auditService) : ControllerBase
    {
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await authService.RegisterAsync(request);

            if (result != "Uspesno")
                return BadRequest(new { poruka = result });

            return Ok(new { poruka = "Uspešna registracija!" });
        }

        [HttpPost("login")]
        [EnableRateLimiting("login_policy")]
        public async Task<IActionResult> Login([FromBody] UserLoginDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var tokenOrError = await authService.LoginAsync(request);

            if (tokenOrError == "Pogrešni kredencijali.")
                return Unauthorized(new { poruka = tokenOrError });

            // 1. BEZBEDNOST: Pakujemo JWT token u HttpOnly Cookie
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true, // Zabranjuje JavaScript-u da čita token (XSS zaštita)
                Secure = true, // Radi samo preko HTTPS konekcije
                SameSite = SameSiteMode.Strict, // Sprečava CSRF napade (zahtevi sa drugih sajtova)
                Expires = DateTime.UtcNow.AddDays(7) // Ističe kad i sam token
            };

            // 2. Šaljemo cookie klijentu
            Response.Cookies.Append("jwt", tokenOrError, cookieOptions);

            // BELEŽIMO U AUDIT LOG DA SE NEKO ULOGOVAO!
            await auditService.LogActionAsync(null, "LOGIN_SUCCESS", $"Korisnik '{request.UsernameOrEmail}' se uspešno prijavio.");

            // 3. Vraćamo samo poruku, BEZ tokena u telu odgovora!
            return Ok(new { poruka = "Uspešna prijava!" });
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            // Proveravamo da li je neko uopšte ulogovan pre nego što logujemo odjavu
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim != null)
            {
                await auditService.LogActionAsync(int.Parse(userIdClaim), "LOGOUT", "Korisnik se odjavio sa sistema.");
            }

            // Brišemo cookie iz pregledača
            Response.Cookies.Delete("jwt");
            return Ok(new { poruka = "Uspešno ste odjavljeni." });
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await authService.ForgotPasswordAsync(request);
            if (result.StartsWith("Greška")) return BadRequest(new { poruka = result });

            return Ok(new { poruka = result });
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await authService.ResetPasswordAsync(request);
            if (result.StartsWith("Greška")) return BadRequest(new { poruka = result });

            return Ok(new { poruka = result });
        }

        [HttpDelete("account")]
        [Authorize] // Katanac - Mora biti ulogovan da bi obrisao svoj nalog!
        public async Task<IActionResult> DeleteAccount([FromBody] DeleteAccountDto request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var result = await authService.DeleteAccountAsync(userId, request.Password);

            if (result.StartsWith("Greška"))
                return BadRequest(new { poruka = result });

            // Slično kao i kod Logout-a, brišemo mu JWT kolačić da ga server automatski "izbaci"
            Response.Cookies.Delete("jwt");

            return Ok(new { poruka = result });
        }
    }
}