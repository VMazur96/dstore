namespace Drajbot.Api.DTOs.Catalogs
{
    public class GameResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? CoverImageUrl { get; set; }

        // Ovde unutar igre spakujemo sve njene proizvode!
        public List<ProductResponseDto> Products { get; set; } = [];
    }
}