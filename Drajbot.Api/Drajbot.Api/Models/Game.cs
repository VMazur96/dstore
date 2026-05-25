using System.ComponentModel.DataAnnotations;

namespace Drajbot.Api.Models
{
    public class Game
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        public string? CoverImageUrl { get; set; } // Slika same igre

        public bool IsActive { get; set; } = true;

        // Navigaciono polje: Jedna igra ima mnogo proizvoda (paketa)
        public ICollection<Product> Products { get; set; } = [];
    }
}