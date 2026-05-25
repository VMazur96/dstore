using Drajbot.Api.DTOs.Fortnite;

namespace Drajbot.Api.Interfaces
{
    public interface IFortniteShopService
    {
        Task<FortniteShopResponseDto> GetDailyShopAsync();
    }
}