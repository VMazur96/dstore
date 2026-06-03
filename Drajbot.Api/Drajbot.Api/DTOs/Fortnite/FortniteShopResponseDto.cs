namespace Drajbot.Api.DTOs.Fortnite
{
    // Glavni objekat koji sadrži generalne informacije o shopu i listu skinova
    public class FortniteShopResponseDto
    {
        public string Date { get; set; } = string.Empty;
        public string CreatorCode { get; set; } = "DRAJBOT"; // Tvoj zakucani kod!
        public DateTime ExpirationDate { get; set; }
        public List<FortniteItemDto> Items { get; set; } = [];
    }

    // Kartica za jedan konkretan skin/item (Ovo crtaš u MTC gridu)
    public class FortniteItemDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Rarity { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public string Section { get; set; } = string.Empty;
        public int VBucksPrice { get; set; }
        public decimal RsdPrice { get; set; } // Ovde ubacujemo tvoju matematiku!
    }
}