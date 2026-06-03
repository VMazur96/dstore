using Drajbot.Api.DTOs.Auth;
using Drajbot.Api.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Drajbot.Api.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController(IAuthService authService) : ControllerBase
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

            // Ako je sve u redu, vraćamo naš JWT token klijentu!
            return Ok(new { token = tokenOrError, poruka = "Uspešna prijava!" });
        }
    }
}