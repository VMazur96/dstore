using Drajbot.Api.Data;
using Drajbot.Api.DTOs.Wishlist;
using Drajbot.Api.Interfaces;
using Drajbot.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Drajbot.Api.Services.Wishlist
{
    public class WishlistService(ApplicationDbContext context) : IWishlistService
    {
        public async Task<string> ToggleWishlistItemAsync(int userId, int productId)
        {
            // Proveravamo da li proizvod uopšte postoji
            var productExists = await context.Products.AnyAsync(p => p.Id == productId);
            if (!productExists) return "Greška: Proizvod ne postoji.";

            // Tražimo da li je korisnik već dodao ovu igru u listu želja
            var existingItem = await context.UserWishlists
                .FirstOrDefaultAsync(w => w.UserId == userId && w.ProductId == productId);

            if (existingItem != null)
            {
                // Ako već postoji, znači da korisnik želi da je IZBRIŠE (Toggle Off)
                context.UserWishlists.Remove(existingItem);
                await context.SaveChangesAsync();
                return "Proizvod je uklonjen iz liste želja.";
            }
            else
            {
                // Ako ne postoji, DODAJEMO je (Toggle On)
                var newItem = new UserWishlist
                {
                    UserId = userId,
                    ProductId = productId
                };
                context.UserWishlists.Add(newItem);
                await context.SaveChangesAsync();
                return "Proizvod je dodat u listu želja!";
            }
        }

        public async Task<IEnumerable<WishlistResponseDto>> GetUserWishlistAsync(int userId)
        {
            return await context.UserWishlists
                .Include(w => w.Product)
                .Where(w => w.UserId == userId)
                .OrderByDescending(w => w.AddedAt)
                .Select(w => new WishlistResponseDto
                {
                    ProductId = w.ProductId,
                    ProductName = w.Product!.Title,
                    CurrentPrice = w.Product.Price,
                    ImageUrl = w.Product.ImageUrl ?? "",
                    AddedAt = w.AddedAt
                }).ToListAsync();
        }
    }
}