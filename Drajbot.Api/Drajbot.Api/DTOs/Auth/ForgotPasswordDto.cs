using System.ComponentModel.DataAnnotations;

namespace Drajbot.Api.DTOs.Auth
{
    public class ForgotPasswordDto
    {
        [Required(ErrorMessage = "Email je obavezan.")]
        [EmailAddress(ErrorMessage = "Nevalidan format email adrese.")]
        public string Email { get; set; } = string.Empty;
    }
}