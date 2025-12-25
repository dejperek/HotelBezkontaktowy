using HotelBezkontaktowy.Data;
using HotelBezkontaktowy.Models.Entities;
using HotelBezkontaktowy.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace HotelBezkontaktowy.Services
{
    public class ReportsService : IReportsService
    {
        private readonly AppDbContext _db;
        public ReportsService(AppDbContext db) { _db = db; }

        public async Task<AdminDashboardVM> GetDashboardAsync(DateTime? from = null, DateTime? to = null, CancellationToken ct = default)
        {
            from ??= DateTime.Today.AddMonths(-6);
            to ??= DateTime.Today.AddMonths(6); // Rozszerzono zakres, aby uwzglêdniæ przysz³e rezerwacje

            // Uwzglêdniamy tylko potwierdzone, zameldowane i zakoñczone rezerwacje (nie pending i cancelled)
            var activeStatuses = new[] { ReservationStatus.Confirmed, ReservationStatus.CheckedIn, ReservationStatus.Completed };

            // Pobierz wszystkie rezerwacje, które nachodz¹ na zakres dat
            var rangeList = await _db.Reservations
                .AsNoTracking()
                .Where(r => activeStatuses.Contains(r.Status) && r.DateFrom < to && r.DateTo > from)
                .Select(r => new { r.DateFrom, r.DateTo })
                .ToListAsync(ct);
            // Oblicz noce dla ka¿dego miesi¹ca - rezerwacja mo¿e obejmowaæ wiele miesiêcy
            var monthlyNights = new List<MonthlyOccupancyVM>();
            
            foreach (var res in rangeList)
            {
                // Iteruj przez ka¿dy dzieñ rezerwacji
                for (var date = res.DateFrom.Date; date < res.DateTo.Date; date = date.AddDays(1))
                {
                    var existing = monthlyNights.FirstOrDefault(m => m.Year == date.Year && m.Month == date.Month);
                    if (existing != null)
                    {
                        existing.Nights++;
                    }
                    else
                    {
                        monthlyNights.Add(new MonthlyOccupancyVM
                        {
                            Year = date.Year,
                            Month = date.Month,
                            Nights = 1
                        });
                    }
                }
            }

            monthlyNights = monthlyNights.OrderBy(x => x.Year).ThenBy(x => x.Month).ToList();

            var revenue = await _db.Reservations.AsNoTracking()
                .Where(r => r.Status != ReservationStatus.Cancelled && r.CreatedAt >= from && r.CreatedAt < to)
                .GroupBy(r => new { r.CreatedAt.Year, r.CreatedAt.Month })
                .Select(g => new RevenueByPeriodVM
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    TotalRevenue = (decimal)g.Sum(r => (double)r.TotalPrice)
                })
                .OrderBy(x => x.Year).ThenBy(x => x.Month)
                .ToListAsync(ct);

            var topTypes = await _db.Reservations.AsNoTracking()
                .Where(r => r.Status != ReservationStatus.Cancelled && r.CreatedAt >= from && r.CreatedAt < to)
                .Include(r => r.Room)!.ThenInclude(rm => rm!.RoomType)
                .GroupBy(r => r.Room!.RoomType!.Name)
                .Select(g => new TopRoomTypeVM
                {
                    RoomType = g.Key,
                    ReservationsCount = g.Count()
                })
                .OrderByDescending(x => x.ReservationsCount)
                .Take(5)
                .ToListAsync(ct);

            return new AdminDashboardVM
            {
                MonthlyOccupancy = monthlyNights,
                Revenue = revenue,
                TopRoomTypes = topTypes
            };
        }
    }
}
