using System.ComponentModel.DataAnnotations;

namespace Drajbot.Api.DTOs.Auth
{
    public class ResetPasswordDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Unesite kod koji ste dobili na email.")]
        public string Token { get; set; } = string.Empty;

        [Required]
        [MinLength(8, ErrorMessage = "Nova lozinka mora imati najmanje 8 karaktera.")]
        public string NewPassword { get; set; } = string.Empty;
    }
}