namespace Drajbot.Api.DTOs.Catalogs
{
    public class ProductResponseDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public decimal? DiscountPrice { get; set; }
        public string Type { get; set; } = string.Empty; // Vratićemo tekst (npr. "InGameCurrency") umesto broja 0
        public string? ImageUrl { get; set; }
    }
}