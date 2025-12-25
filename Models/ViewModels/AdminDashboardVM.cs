namespace HotelBezkontaktowy.Models.ViewModels
{
    public class AdminDashboardVM
    {
        public List<MonthlyOccupancyVM> MonthlyOccupancy { get; set; } = new();
        public List<RevenueByPeriodVM> Revenue { get; set; } = new();
        public List<TopRoomTypeVM> TopRoomTypes { get; set; } = new();
    }

    public class MonthlyOccupancyVM
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public int Nights { get; set; }
    }

    public class RevenueByPeriodVM
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public decimal TotalRevenue { get; set; }
    }

    public class TopRoomTypeVM
    {
        public string RoomType { get; set; } = string.Empty;
        public int ReservationsCount { get; set; }
    }
}
