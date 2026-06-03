using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Drajbot.Api.Models
{
    public class Order
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; } // Ko je kupio
        [ForeignKey("UserId")]
        public User? User { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalPrice { get; set; }

        // Ovde korisnik upisuje svoj Epic Games Nick da bismo mu poslali gift!
        [MaxLength(100)]
        public string? GameUsername { get; set; }

        // Status porudžbine: "Pending" (Čeka uplatu/48h), "Completed" (Isporučeno), "Cancelled"
        public string Status { get; set; } = "Pending";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Veza ka svim artiklima iz korpe
        public ICollection<OrderItem> OrderItems { get; set; } = [];
    }
}