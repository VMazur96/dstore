using System.ComponentModel.DataAnnotations;

namespace Drajbot.Api.DTOs.Reviews
{
    public class ReviewCreateDto
    {
        [Required]
        [Range(1, 5, ErrorMessage = "Ocena mora biti između 1 i 5.")]
        public int Rating { get; set; }

        public string? Comment { get; set; } // Opciono polje!
    }
}