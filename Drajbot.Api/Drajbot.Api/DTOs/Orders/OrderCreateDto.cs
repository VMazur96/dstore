using System.ComponentModel.DataAnnotations;

namespace Drajbot.Api.DTOs.Orders
{
    public class OrderCreateDto
    {
        [MaxLength(500)]
        public string? AccountDetails { get; set; } // Ovde frontend šalje spakovan tekst

        [Required]
        [Range(typeof(bool), "true", "true", ErrorMessage = "Morate potvrditi da ste proverili cene sa administratorom pre kreiranja uplatnice.")]
        public bool PricesConfirmed { get; set; } // Kvačica na frontendu

        [Required]
        [MinLength(1, ErrorMessage = "Korpa ne sme biti prazna.")]
        public List<OrderItemCreateDto> Items { get; set; } = [];
    }
}