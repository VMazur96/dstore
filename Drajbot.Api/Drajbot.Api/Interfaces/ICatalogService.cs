using Drajbot.Api.DTOs.Catalogs;

namespace Drajbot.Api.Interfaces
{
    public interface ICatalogService
    {
        Task<string> AddGameAsync(GameCreateDto request);
        Task<string> AddProductAsync(ProductCreateDto request);
        Task<IEnumerable<GameResponseDto>> GetGamesAsync();
        Task<IEnumerable<ProductResponseDto>> GetProductsByGameAsync(int gameId);
    }
}