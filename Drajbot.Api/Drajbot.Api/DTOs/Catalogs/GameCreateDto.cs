using System.ComponentModel.DataAnnotations;

namespace Drajbot.Api.DTOs.Catalogs
{
    public class GameCreateDto
    {
        [Required(ErrorMessage = "Ime igre (kategorije) je obavezno.")]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        public string? CoverImageUrl { get; set; }
    }
}