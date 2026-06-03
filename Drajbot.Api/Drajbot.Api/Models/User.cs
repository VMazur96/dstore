using System.ComponentModel.DataAnnotations;
using Drajbot.Api.Enums;

namespace Drajbot.Api.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(150)]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        // KORISTIMO ENUMERACIJE!
        public LoyaltyRank LoyaltyRank { get; set; } = LoyaltyRank.Bronze;
        public Role Role { get; set; } = Role.User;

        public string? AvatarUrl { get; set; }

        public bool IsActive { get; set; } = true;

        [MaxLength(10)]
        public string? PasswordResetToken { get; set; }
        public DateTime? ResetTokenExpires { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}