using Drajbot.Api.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Drajbot.Api.Controllers
{
    [ApiController]
    [Route("api/wishlist")]
    [Authorize] // Katanac! Samo ulogovani korisnici imaju listu želja
    public class WishlistController(IWishlistService wishlistService) : ControllerBase
    {
        [HttpPost("toggle/{productId}")]
        public async Task<IActionResult> ToggleItem(int productId)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var result = await wishlistService.ToggleWishlistItemAsync(userId, productId);

            if (result.StartsWith("Greška")) return BadRequest(new { poruka = result });

            return Ok(new { poruka = result });
        }

        [HttpGet]
        public async Task<IActionResult> GetMyWishlist()
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var items = await wishlistService.GetUserWishlistAsync(userId);
            return Ok(items);
        }
    }
}