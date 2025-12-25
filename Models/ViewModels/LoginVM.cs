using System.ComponentModel.DataAnnotations;

namespace HotelBezkontaktowy.Models.ViewModels
{
    public class LoginVM
    {
        [Required(ErrorMessage = "Email jest wymagany")]
        [EmailAddress(ErrorMessage = "Nieprawid³owy format email")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Has³o jest wymagane")]
        [DataType(DataType.Password)]
        [Display(Name = "Has³o")]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Zapamiêtaj mnie")]
        public bool RememberMe { get; set; }

        public string? ReturnUrl { get; set; }
    }
}
