using Drajbot.Api.Data;
using Drajbot.Api.DTOs.Catalogs;
using Drajbot.Api.Interfaces;
using Drajbot.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Drajbot.Api.Services.Catalogs
{
    public class CatalogService(ApplicationDbContext context) : ICatalogService
    {
        public async Task<string> AddGameAsync(GameCreateDto request)
        {
            var game = new Game
            {
                Name = request.Name,
                Description = request.Description,
                CoverImageUrl = request.CoverImageUrl,
                IsActive = true
            };

            context.Games.Add(game);
            await context.SaveChangesAsync();
            return "Igra (kategorija) je uspešno dodata!";
        }

        public async Task<string> AddProductAsync(ProductCreateDto request)
        {
            // Provera da li igra u koju pokušavamo da ubacimo paket uopšte postoji!
            var gameExists = await context.Games.AnyAsync(g => g.Id == request.GameId);
            if (!gameExists)
                return "Greška: Izabrana igra ne postoji u sistemu.";

            var product = new Product
            {
                GameId = request.GameId,
                Title = request.Title,
                Description = request.Description,
                Price = request.Price,
                DiscountPrice = request.DiscountPrice,
                Type = request.Type,
                ImageUrl = request.ImageUrl,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            context.Products.Add(product);
            await context.SaveChangesAsync();
            return "Proizvod je uspešno dodat u ponudu!";
        }

        // Vraća samo naslovne kartice igara (PUBG, Fortnite...)
        public async Task<IEnumerable<GameResponseDto>> GetGamesAsync()
        {
            return await context.Games
                .Where(g => g.IsActive)
                .Select(g => new GameResponseDto
                {
                    Id = g.Id,
                    Name = g.Name,
                    Description = g.Description,
                    CoverImageUrl = g.CoverImageUrl,
                    Products = new List<ProductResponseDto>() // Ostavljamo prazno da ubrzamo učitavanje!
                })
                .ToListAsync();
        }

        // Vraća samo grid ponuda kada se klikne na konkretnu igru (60 UC, 1000 V-Bucks...)
        public async Task<IEnumerable<ProductResponseDto>> GetProductsByGameAsync(int gameId)
        {
            return await context.Products
                .Where(p => p.GameId == gameId && p.IsActive)
                .Select(p => new ProductResponseDto
                {
                    Id = p.Id,
                    Title = p.Title,
                    Description = p.Description,
                    Price = p.Price,
                    DiscountPrice = p.DiscountPrice,
                    Type = p.Type.ToString(),
                    ImageUrl = p.ImageUrl
                })
                .ToListAsync();
        }
    }
}