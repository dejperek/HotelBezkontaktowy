using System.ComponentModel.DataAnnotations;

namespace HotelBezkontaktowy.Models.Entities
{
    public class Amenity
    {
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(300)]
        public string? Icon { get; set; }

        public ICollection<RoomAmenity> RoomAmenities { get; set; } = new List<RoomAmenity>();
    }
}
