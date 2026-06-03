using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Drajbot.Api.Models
{
    public class UserWishlist
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

        public DateTime AddedAt { get; set; } = DateTime.UtcNow;
    }
}