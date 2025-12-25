using System.ComponentModel.DataAnnotations;

namespace HotelBezkontaktowy.Models.Entities
{
    public class Room
    {
        public int Id { get; set; }

        [Required, MaxLength(20)]
        public string RoomNumber { get; set; } = string.Empty;

        public int RoomTypeId { get; set; }
        public RoomType? RoomType { get; set; }

        [Range(1, 20)]
        public int Capacity { get; set; }

        [Range(0, 100000)]
        public decimal PricePerNight { get; set; }

        public bool IsActive { get; set; } = true;

        public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
        public ICollection<RoomAmenity> RoomAmenities { get; set; } = new List<RoomAmenity>();
    }
}
