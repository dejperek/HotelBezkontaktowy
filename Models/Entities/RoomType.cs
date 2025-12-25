using System.ComponentModel.DataAnnotations;

namespace HotelBezkontaktowy.Models.Entities
{
    public class RoomType
    {
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        public ICollection<Room> Rooms { get; set; } = new List<Room>();
    }
}
