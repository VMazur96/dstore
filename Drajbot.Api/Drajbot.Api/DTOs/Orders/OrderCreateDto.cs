using System.ComponentModel.DataAnnotations;

namespace Drajbot.Api.DTOs.Orders
{
    public class OrderCreateDto
    {
        [Required(ErrorMessage = "Epic Games korisničko ime je obavezno zbog isporuke.")]
        [MaxLength(100)]
        public string GameUsername { get; set; } = string.Empty; // Tražimo mu Epic Nick!

        [Required]
        [MinLength(1, ErrorMessage = "Korpa ne sme biti prazna.")]
        public List<OrderItemCreateDto> Items { get; set; } = []; // Lista onoga što je kliknuo
    }
}