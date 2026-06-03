namespace Drajbot.Api.DTOs.Orders
{
    public class OrderItemResponseDto
    {
        public int Id { get; set; }
        public int? ProductId { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
}