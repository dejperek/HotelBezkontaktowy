using System.ComponentModel.DataAnnotations;
using HotelBezkontaktowy.Data;

namespace HotelBezkontaktowy.Models.Entities
{
    public enum ReservationStatus
    {
        Pending = 0,
        Confirmed = 1,
        CheckedIn = 2,
        Completed = 3,
        Cancelled = 4
    }

    public class Reservation
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        public int RoomId { get; set; }

        [Required]
        public DateTime DateFrom { get; set; }

        [Required]
        public DateTime DateTo { get; set; }

        [Range(1, 20)]
        public int GuestsCount { get; set; }

        public ReservationStatus Status { get; set; } = ReservationStatus.Pending;

        [Range(0, 1000000)]
        public decimal TotalPrice { get; set; }

        [MaxLength(64)]
        public string? AccessToken { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Room? Room { get; set; }
        public ApplicationUser? User { get; set; }
    }
}
