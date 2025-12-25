using System.ComponentModel.DataAnnotations;
using HotelBezkontaktowy.Models.Entities;

namespace HotelBezkontaktowy.Models.ViewModels
{
    public class ReservationCreateVM : IValidatableObject
    {
        [Required]
        public int RoomId { get; set; }

        [Required, DataType(DataType.Date)]
        public DateTime DateFrom { get; set; }

        [Required, DataType(DataType.Date)]
        public DateTime DateTo { get; set; }

        [Range(1, 20)]
        public int GuestsCount { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (DateFrom.Date >= DateTo.Date)
            {
                yield return new ValidationResult("Data od musi byæ wczeœniejsza ni¿ data do.", new[] { nameof(DateFrom), nameof(DateTo) });
            }
            if (DateFrom.Date < DateTime.Today)
            {
                yield return new ValidationResult("Nie mo¿na rezerwowaæ w przesz³oœci.", new[] { nameof(DateFrom) });
            }
            if ((DateTo - DateFrom).TotalDays < 1)
            {
                yield return new ValidationResult("Minimalna d³ugoœæ pobytu to 1 noc.", new[] { nameof(DateTo) });
            }
            if ((DateTo - DateFrom).TotalDays > 30)
            {
                yield return new ValidationResult("Maksymalna d³ugoœæ pobytu to 30 nocy.", new[] { nameof(DateTo) });
            }
        }
    }
}
