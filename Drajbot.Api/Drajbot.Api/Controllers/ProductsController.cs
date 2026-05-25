using Drajbot.Api.DTOs.Catalogs;
using Drajbot.Api.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Drajbot.Api.Controllers
{
    [ApiController]
    [Route("api/catalog")] // Promenjeno u catalog da bude logičnije
    public class ProductsController(ICatalogService catalogService) : ControllerBase
    {
        // Samo Admin može da napravi novu igru (npr. Fortnite, PUBG)
        [HttpPost("games")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateGame([FromBody] GameCreateDto request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = await catalogService.AddGameAsync(request);
            return Ok(new { poruka = result });
        }

        // Samo Admin može da dodaje pakete unutar igre (npr. 1000 V-Bucks)
        [HttpPost("items")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateProduct([FromBody] ProductCreateDto request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = await catalogService.AddProductAsync(request);

            if (result.StartsWith("Greška"))
                return BadRequest(new { poruka = result });

            return Ok(new { poruka = result });
        }

        // 1. Prikaz početne strane (Grid svih Igara)
        [HttpGet("games")]
        public async Task<IActionResult> GetGames()
        {
            var games = await catalogService.GetGamesAsync();
            return Ok(games);
        }

        // 2. Prikaz paketa kada se klikne na Igru (npr. ID 1 za Fortnite)
        [HttpGet("games/{gameId}/items")]
        public async Task<IActionResult> GetItemsForGame(int gameId)
        {
            var items = await catalogService.GetProductsByGameAsync(gameId);
            return Ok(items);
        }
    }
}