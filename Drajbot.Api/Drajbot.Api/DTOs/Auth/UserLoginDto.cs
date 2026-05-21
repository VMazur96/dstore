using System.ComponentModel.DataAnnotations;

namespace Drajbot.Api.DTOs.Auth
{
    public class UserLoginDto
    {
        // Više ne tražimo striktno EmailAddress, već može biti bilo koji tekst
        [Required(ErrorMessage = "Korisničko ime ili email su obavezni.")]
        public string UsernameOrEmail { get; set; } = string.Empty;

        [Required(ErrorMessage = "Lozinka je obavezna.")]
        public string Password { get; set; } = string.Empty;
    }
}