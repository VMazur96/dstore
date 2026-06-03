namespace Drajbot.Api.DTOs.Orders
{
    public class OrderResponseDto
    {
        public int Id { get; set; }
        public decimal TotalPrice { get; set; }
        public string GameUsername { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public List<OrderItemResponseDto> Items { get; set; } = [];
    }
}