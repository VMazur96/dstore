using Drajbot.Api.Data;
using Drajbot.Api.DTOs.Reviews;
using Drajbot.Api.Interfaces;
using Drajbot.Api.Models;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Text.RegularExpressions;

namespace Drajbot.Api.Services.Reviews
{
    public class ReviewService(ApplicationDbContext context) : IReviewService
    {
        // Pomoćna metoda za validaciju i XSS zaštitu
        private static string? ValidateAndSanitizeComment(string? comment)
        {
            if (string.IsNullOrWhiteSpace(comment)) return string.Empty;

            // Zabrana spam-a (npr. "aaaaa") - ako ima 5 istih karaktera zaredom, baci grešku
            if (Regex.IsMatch(comment, @"(.)\1{4,}"))
                throw new Exception("Komentar izgleda kao spam (previše istih slova u nizu).");

            // XSS ZAŠTITA: Pretvara <script> u bezbedni tekst &lt;script&gt;
            return WebUtility.HtmlEncode(comment);
        }

        public async Task<string> AddProductReviewAsync(int userId, int productId, ReviewCreateDto request)
        {
            try
            {
                string safeComment = ValidateAndSanitizeComment(request.Comment) ?? string.Empty;

                // SPECIFIKACIJA: Provera "Verifikovane kupovine"
                // Proveravamo da li u OrderItems postoji ovaj ProductId vezan za ovog Usera a da je Order Completed
                bool hasPurchased = await context.OrderItems
                    .Include(oi => oi.Order)
                    .AnyAsync(oi => oi.Order!.UserId == userId
                                 && oi.Order.Status == "Completed"
                                 && oi.ProductId == productId);

                var review = new ProductReview
                {
                    UserId = userId,
                    ProductId = productId,
                    Rating = request.Rating,
                    Comment = safeComment,
                    IsVerifiedPurchase = hasPurchased,
                    IsApproved = false // Sakriveno po defaultu!
                };

                context.ProductReviews.Add(review);
                await context.SaveChangesAsync();

                return "Uspešno! Vaša recenzija čeka odobrenje moderatora.";
            }
            catch (Exception ex) { return $"Greška: {ex.Message}"; }
        }

        public async Task<string> AddSiteReviewAsync(int userId, ReviewCreateDto request)
        {
            try
            {
                string safeComment = ValidateAndSanitizeComment(request.Comment) ?? string.Empty;

                var review = new SiteReview
                {
                    UserId = userId,
                    Rating = request.Rating,
                    Comment = safeComment,
                    IsApproved = false
                };

                context.SiteReviews.Add(review);
                await context.SaveChangesAsync();

                return "Uspešno! Vaš utisak o sajtu je poslat na odobrenje.";
            }
            catch (Exception ex) { return $"Greška: {ex.Message}"; }
        }

        // Prikaz publici (Zato vuče samo IsApproved == true)
        public async Task<IEnumerable<ReviewResponseDto>> GetApprovedProductReviewsAsync(int productId)
        {
            return await context.ProductReviews
                .Include(pr => pr.User)
                .Where(pr => pr.ProductId == productId && pr.IsApproved == true)
                .OrderByDescending(pr => pr.CreatedAt)
                .Select(pr => new ReviewResponseDto
                {
                    Id = pr.Id,
                    Username = pr.User!.Username, // Samo username
                    Rating = pr.Rating,
                    Comment = pr.Comment,
                    IsVerifiedPurchase = pr.IsVerifiedPurchase,
                    CreatedAt = pr.CreatedAt
                }).ToListAsync();
        }

        // Admin metoda za odobravanje/sakrivanje recenzija
        public async Task<string> ToggleReviewVisibilityAsync(int reviewId, string type)
        {
            if (type.Equals("product", StringComparison.OrdinalIgnoreCase))
            {
                var review = await context.ProductReviews.FindAsync(reviewId);
                if (review == null) return "Nije pronađeno.";
                review.IsApproved = !review.IsApproved; // Obrće iz false u true ili obrnuto
            }
            else
            {
                var review = await context.SiteReviews.FindAsync(reviewId);
                if (review == null) return "Nije pronađeno.";
                review.IsApproved = !review.IsApproved;
            }

            await context.SaveChangesAsync();
            return "Status vidljivosti recenzije je uspešno promenjen.";
        }
    }
}