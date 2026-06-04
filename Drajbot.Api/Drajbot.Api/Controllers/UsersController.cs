using Drajbot.Api.DTOs.Auth;
using Drajbot.Api.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Drajbot.Api.Controllers
{
    [ApiController]
    [Route("api/users")]
    [Authorize(Roles = "Admin")] // SAMO TI IMAŠ PRISTUP OVOME!
    public class UsersController(IAuthService authService, IAuditService auditService) : ControllerBase
    {
        [HttpPut("{id}/force-credentials")]
        public async Task<IActionResult> ForceChange(int id, [FromBody] AdminForceChangeDto request)
        {
            var result = await authService.AdminForceChangeCredentialsAsync(id, request.NewEmail, request.NewPassword);

            if (result.StartsWith("Greška"))
                return BadRequest(new { poruka = result });

            // Zapiši u Audit Log da si iskoristio Admin moći!
            int adminId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            await auditService.LogActionAsync(adminId, "ADMIN_FORCE_CHANGE", $"Admin je nasilno promenio podatke korisniku ID: {id}.");

            return Ok(new { poruka = result });
        }
    }
}