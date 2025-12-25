using System.ComponentModel.DataAnnotations;

namespace HotelBezkontaktowy.Models.ViewModels
{
    public class RoomSearchVM : IValidatableObject
    {
        [DataType(DataType.Date)]
        public DateTime? DateFrom { get; set; }

        [DataType(DataType.Date)]
        public DateTime? DateTo { get; set; }

        [Range(1, 20)]
        public int? Guests { get; set; }

        public int? RoomTypeId { get; set; }

        [Range(0, 100000)]
        public decimal? MinPrice { get; set; }

        [Range(0, 100000)]
        public decimal? MaxPrice { get; set; }

        public List<RoomListItemVM> Results { get; set; } = new();

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (DateFrom.HasValue && DateTo.HasValue)
            {
                if (DateFrom.Value.Date >= DateTo.Value.Date)
                {
                    yield return new ValidationResult("Data od musi byæ wczeœniejsza ni¿ data do.", new[] { nameof(DateFrom), nameof(DateTo) });
                }
            }
            if (MinPrice.HasValue && MaxPrice.HasValue && MinPrice > MaxPrice)
            {
                yield return new ValidationResult("Minimalna cena nie mo¿e byæ wy¿sza ni¿ maksymalna.", new[] { nameof(MinPrice), nameof(MaxPrice) });
            }
        }
    }

    public class RoomListItemVM
    {
        public int Id { get; set; }
        public string RoomNumber { get; set; } = string.Empty;
        public string RoomType { get; set; } = string.Empty;
        public int Capacity { get; set; }
        public decimal PricePerNight { get; set; }
        public bool IsActive { get; set; }
    }
}
