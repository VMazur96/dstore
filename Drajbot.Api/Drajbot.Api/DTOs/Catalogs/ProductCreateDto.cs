using System.ComponentModel.DataAnnotations;
using Drajbot.Api.Enums;

namespace Drajbot.Api.DTOs.Catalogs
{
    public class ProductCreateDto
    {
        [Required]
        public int GameId { get; set; } // ID igre kojoj pripada (npr. ID od Fortnite-a)

        [Required(ErrorMessage = "Naslov paketa je obavezan.")]
        [MaxLength(100)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Cena je obavezna.")]
        public decimal Price { get; set; }

        public decimal? DiscountPrice { get; set; }

        [Required]
        public ProductType Type { get; set; } // Očekuje broj (0 = Valuta, 1 = Skin...)

        public string? ImageUrl { get; set; }
    }
}