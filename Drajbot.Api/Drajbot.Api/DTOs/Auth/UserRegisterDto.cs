using System.ComponentModel.DataAnnotations;

namespace Drajbot.Api.DTOs.Auth
{
    // Implementiramo IValidatableObject za napredne logičke provere
    public class UserRegisterDto : IValidatableObject
    {
        [Required(ErrorMessage = "Ime je obavezno.")]
        [MaxLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Prezime je obavezno.")]
        [MaxLength(50)]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Korisničko ime je obavezno.")]
        [MaxLength(30)]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email je obavezan.")]
        [EmailAddress(ErrorMessage = "Nevalidan format email adrese.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Lozinka je obavezna.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$",
            ErrorMessage = "Lozinka mora imati min 8 karaktera, jedno veliko slovo, malo slovo, broj i specijalni znak.")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Potvrda lozinke je obavezna.")]
        [Compare("Password", ErrorMessage = "Lozinke se ne poklapaju.")]
        public string ConfirmPassword { get; set; } = string.Empty;

        // OVDE DODAJEMO TVOJU NAPREDNU LOGIKU!
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!string.IsNullOrEmpty(Password))
            {
                // 1. Lozinka ne sme da sadrži korisničko ime (nebitno da li su velika ili mala slova)
                if (!string.IsNullOrEmpty(Username) && Password.Contains(Username, StringComparison.OrdinalIgnoreCase))
                {
                    yield return new ValidationResult("Lozinka ne sme da sadrži vaše korisničko ime.", [nameof(Password)]);
                }

                // 2. Lozinka ne sme da sadrži deo emaila pre znaka @ (npr. ako je "vlada@gmail.com", lozinka ne sme imati "vlada")
                if (!string.IsNullOrEmpty(Email) && Email.Contains('@'))
                {
                    var emailPrefix = Email.Split('@')[0];
                    if (Password.Contains(emailPrefix, StringComparison.OrdinalIgnoreCase))
                    {
                        yield return new ValidationResult("Lozinka ne sme da sadrži deo vaše email adrese.", [nameof(Password)]);
                    }
                }
            }
        }
    }
}