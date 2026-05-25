using Drajbot.Api.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Drajbot.Api.Controllers
{
    [ApiController]
    [Route("api/fortnite")]
    public class FortniteController(IFortniteShopService fortniteService) : ControllerBase
    {
        [HttpGet("shop")]
        public async Task<IActionResult> GetDailyShop()
        {
            var shop = await fortniteService.GetDailyShopAsync();
            return Ok(shop);
        }
    }
}