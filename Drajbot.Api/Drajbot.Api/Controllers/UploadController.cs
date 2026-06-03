using Drajbot.Api.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Drajbot.Api.Controllers
{
    [ApiController]
    [Route("api/upload")]
    [Authorize] // Katanac! Ne može anonimni haker da nam puni disk smećem.
    public class UploadController(IUploadService uploadService) : ControllerBase
    {
        [HttpPost("image")]
        public async Task<IActionResult> UploadImage(IFormFile file, [FromQuery] string folder = "general")
        {
            var result = await uploadService.UploadImageAsync(file, folder);

            if (result.StartsWith("Greška"))
                return BadRequest(new { poruka = result });

            return Ok(new { url = result, poruka = "Slika je uspešno sačuvana!" });
        }
    }
}