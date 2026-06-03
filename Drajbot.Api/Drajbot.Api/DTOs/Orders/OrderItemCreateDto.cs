namespace Drajbot.Api.DTOs.Orders
{
    public class OrderItemCreateDto
    {
        public int? ProductId { get; set; } // Ako kupuje V-Buckse iz naše baze
        public string ItemName { get; set; } = string.Empty; // Npr. "Slim Shady Bundle"
        public decimal Price { get; set; }
        public int Quantity { get; set; } = 1;
    }
}