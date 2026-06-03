using Drajbot.Api.DTOs.Orders;

namespace Drajbot.Api.Interfaces
{
    public interface IOrderService
    {
        Task<string> CreateOrderAsync(int userId, OrderCreateDto request);
        Task<IEnumerable<OrderResponseDto>> GetUserOrdersAsync(int userId); // Za obične korisnike
        Task<IEnumerable<OrderResponseDto>> GetAllOrdersAsync(); // Snajper ruta za tebe (Admina)
        Task<string> UpdateOrderStatusAsync(int orderId, string newStatus); // Za menjanje statusa nakon isporuke
    }
}