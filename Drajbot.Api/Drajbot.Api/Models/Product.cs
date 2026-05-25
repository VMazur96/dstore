using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Drajbot.Api.Enums;

namespace Drajbot.Api.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        // Strani ključ (Foreign Key) - Govori nam kojoj igri pripada ovaj paket
        [Required]
        public int GameId { get; set; }

        [ForeignKey("GameId")]
        public Game? Game { get; set; } // Poveznica ka tabeli iznad

        [Required]
        [MaxLength(100)]
        public string Title { get; set; } = string.Empty; // npr. "1000 V-Bucks"

        [MaxLength(500)]
        public string? Description { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")] // Precizno čuvanje valuta
        public decimal Price { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? DiscountPrice { get; set; } // Opciono: Ako je paket na akciji

        public ProductType Type { get; set; } = ProductType.InGameCurrency;

        public string? ImageUrl { get; set; } // Slika samog paketa (npr. ikonica V-Bucksa)

        // Ovo omogućava da Admin privremeno sakrije paket iz prodavnice (npr. ako ga nema na stanju)
        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}