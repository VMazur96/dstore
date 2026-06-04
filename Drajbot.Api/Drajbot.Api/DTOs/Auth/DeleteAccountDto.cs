using System.ComponentModel.DataAnnotations;

namespace Drajbot.Api.DTOs.Auth
{
    public class DeleteAccountDto
    {
        [Required(ErrorMessage = "Morate uneti lozinku da biste potvrdili brisanje naloga.")]
        public string Password { get; set; } = string.Empty;
    }
}