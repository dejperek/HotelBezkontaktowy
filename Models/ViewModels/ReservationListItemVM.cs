namespace HotelBezkontaktowy.Models.ViewModels
{
    public class ReservationListItemVM
    {
        public int Id { get; set; }
        public string RoomNumber { get; set; } = string.Empty;
        public string RoomType { get; set; } = string.Empty;
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public int GuestsCount { get; set; }
        public string Status { get; set; } = string.Empty;
        public decimal TotalPrice { get; set; }
    }
}
