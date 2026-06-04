using System.ComponentModel.DataAnnotations;

namespace Drajbot.Api.Models
{
    public class BlogPost
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Content { get; set; } = string.Empty; // Ovde ćemo čuvati HTML tekst

        public string? ImageUrl { get; set; } // Slika za naslovnu bloga

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}