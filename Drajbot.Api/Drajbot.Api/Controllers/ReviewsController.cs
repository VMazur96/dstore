using Drajbot.Api.DTOs.Reviews;
using Drajbot.Api.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Drajbot.Api.Controllers
{
    [ApiController]
    [Route("api/reviews")]
    // Bilo je: public class ReviewsController(IReviewService reviewService) : ControllerBase
    public class ReviewsController(IReviewService reviewService, IAuditService auditService) : ControllerBase
    {
        // 1. Ostavljanje recenzije za specifičnu IGRU (Zaštićeno: Korisnik)
        [HttpPost("product/{productId}")]
        [Authorize]
        public async Task<IActionResult> AddProductReview(int productId, [FromBody] ReviewCreateDto request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var result = await reviewService.AddProductReviewAsync(userId, productId, request);

            if (result.StartsWith("Greška")) return BadRequest(new { poruka = result });

            await auditService.LogActionAsync(userId, "REVIEW_ADDED", $"Korisnik je ostavio recenziju (Ocena: {request.Rating}) za proizvod ID: {productId}.");

            return Ok(new { poruka = result });
        }

        // 2. Pregled ODOBRENIH recenzija za igru (Javno)
        [HttpGet("product/{productId}")]
        public async Task<IActionResult> GetProductReviews(int productId)
        {
            var reviews = await reviewService.GetApprovedProductReviewsAsync(productId);
            return Ok(reviews);
        }

        // 3. Admin / Moderator kontrola: Odobravanje ili sakrivanje
        [HttpPatch("{id}/toggle-visibility")]
        [Authorize(Roles = "Admin,Moderator")]
        public async Task<IActionResult> ToggleVisibility(int id, [FromQuery] string type = "product")
        {
            var result = await reviewService.ToggleReviewVisibilityAsync(id, type);
            if (result == "Nije pronađeno.") return NotFound(new { poruka = result });
            return Ok(new { poruka = result });
        }
    }
}