using Drajbot.Api.DTOs.Orders;
using Drajbot.Api.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Drajbot.Api.Controllers
{
    [ApiController]
    [Route("api/orders")]
    [Authorize] // Katanac za celu klasu
    public class OrdersController(IOrderService orderService) : ControllerBase
    {
        // 1. Pravljenje porudžbine (Checkout)
        [HttpPost]
        public async Task<IActionResult> Checkout([FromBody] OrderCreateDto request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null) return Unauthorized("Niste ulogovani.");

            int userId = int.Parse(userIdClaim);

            var result = await orderService.CreateOrderAsync(userId, request);
            return Ok(new { poruka = result });
        }

        // 2. Korisnička ruta: Korisnik gleda svoje stare kupovine
        [HttpGet("my")]
        public async Task<IActionResult> GetMyOrders()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null) return Unauthorized();

            int userId = int.Parse(userIdClaim);
            var orders = await orderService.GetUserOrdersAsync(userId);
            return Ok(orders);
        }

        // 3. Admin ruta: Pregled apsolutno svih porudžbina na celom sajtu
        [HttpGet("admin/all")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllOrders()
        {
            var orders = await orderService.GetAllOrdersAsync();
            return Ok(orders);
        }

        // 4. Admin ruta: Menjanje statusa (Npr. u "Completed" nakon slanja gifta)
        [HttpPut("admin/{id}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateStatus(int id, [FromQuery] string status)
        {
            var result = await orderService.UpdateOrderStatusAsync(id, status);
            if (result.StartsWith("Greška")) return BadRequest(new { poruka = result });

            return Ok(new { poruka = result });
        }
    }
}