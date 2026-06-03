using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Drajbot.Api.Models
{
    public class OrderItem
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int OrderId { get; set; }
        [ForeignKey("OrderId")]
        public Order? Order { get; set; }

        // Opciono: Ako kupuje V-Buckse, ovde pamtimo koji je to paket iz naše baze
        public int? ProductId { get; set; }
        [ForeignKey("ProductId")]
        public Product? Product { get; set; }

        // OBAVEZNO: Pamtimo tačno ime artikla. Ako kupuje Skin iz Fortnite shopa,
        // on nema "ProductId", već ovde direktno piše npr. "Slim Shady Skin".
        [Required]
        [MaxLength(150)]
        public string ItemName { get; set; } = string.Empty;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        public int Quantity { get; set; } = 1;
    }
}