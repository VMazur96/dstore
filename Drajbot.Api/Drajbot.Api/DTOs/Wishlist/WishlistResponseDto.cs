namespace Drajbot.Api.DTOs.Wishlist
{
    public class WishlistResponseDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public decimal CurrentPrice { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public DateTime AddedAt { get; set; }
    }
}