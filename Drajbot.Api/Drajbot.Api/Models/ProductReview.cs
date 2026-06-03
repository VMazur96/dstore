using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Drajbot.Api.Models
{
    public class ProductReview
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public User? User { get; set; }

        [Required]
        public int ProductId { get; set; }
        [ForeignKey("ProductId")]
        public Product? Product { get; set; }

        // BEZBEDNOST: Ocena strogo od 1 do 5!
        [Required]
        [Range(1, 5, ErrorMessage = "Ocena mora biti između 1 i 5.")]
        public int Rating { get; set; }

        [MaxLength(1000)]
        public string Comment { get; set; } = string.Empty;

        // Ove dve stvari server dodeljuje sam!
        public bool IsVerifiedPurchase { get; set; } = false; // Da li je korisnik zaista kupio ovo?
        public bool IsApproved { get; set; } = false; // Admin mora da odobri

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}