using System.ComponentModel.DataAnnotations;

namespace HotelBezkontaktowy.Models.ViewModels
{
    public class RegisterVM
    {
        [Required(ErrorMessage = "Email jest wymagany")]
        [EmailAddress(ErrorMessage = "Nieprawid³owy format email")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Has³o jest wymagane")]
        [StringLength(100, ErrorMessage = "Has³o musi mieæ co najmniej {2} znaków.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Has³o")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Potwierdzenie has³a jest wymagane")]
        [DataType(DataType.Password)]
        [Display(Name = "PotwierdŸ has³o")]
        [Compare("Password", ErrorMessage = "Has³a nie s¹ identyczne.")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
