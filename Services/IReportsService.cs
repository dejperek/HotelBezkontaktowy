using HotelBezkontaktowy.Models.ViewModels;

namespace HotelBezkontaktowy.Services
{
    public interface IReportsService
    {
        Task<AdminDashboardVM> GetDashboardAsync(DateTime? from = null, DateTime? to = null, CancellationToken ct = default);
    }
}
