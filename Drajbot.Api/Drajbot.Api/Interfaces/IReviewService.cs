using Drajbot.Api.DTOs.Reviews;

namespace Drajbot.Api.Interfaces
{
    public interface IReviewService
    {
        Task<string> AddProductReviewAsync(int userId, int productId, ReviewCreateDto request);
        Task<string> AddSiteReviewAsync(int userId, ReviewCreateDto request);
        Task<IEnumerable<ReviewResponseDto>> GetApprovedProductReviewsAsync(int productId);
        Task<string> ToggleReviewVisibilityAsync(int reviewId, string type); // Za Admina
    }
}