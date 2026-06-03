using Drajbot.Api.DTOs.Wishlist;

namespace Drajbot.Api.Interfaces
{
    public interface IWishlistService
    {
        // Ova metoda radi kao prekidač: Ako igra nije u listi - dodaje je. Ako jeste - briše je.
        Task<string> ToggleWishlistItemAsync(int userId, int productId);
        Task<IEnumerable<WishlistResponseDto>> GetUserWishlistAsync(int userId);
    }
}