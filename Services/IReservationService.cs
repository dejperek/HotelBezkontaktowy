using HotelBezkontaktowy.Models.Entities;
using HotelBezkontaktowy.Models.ViewModels;

namespace HotelBezkontaktowy.Services
{
    public interface IReservationService
    {
        Task<bool> HasConflictsAsync(int roomId, DateTime from, DateTime to, int? excludeReservationId = null, CancellationToken ct = default);
        Task<int> CreateAsync(string userId, ReservationCreateVM vm, CancellationToken ct = default);
        Task<List<ReservationListItemVM>> GetUserReservationsAsync(string userId, CancellationToken ct = default);
        Task<string?> GenerateAccessTokenAsync(int reservationId, string userId, CancellationToken ct = default);
        Task<bool> ChangeStatusAsync(int reservationId, ReservationStatus status, CancellationToken ct = default);
        Task<Reservation?> GetByAccessTokenAsync(string token, CancellationToken ct = default);
        Task<List<ReservationListItemVM>> GetAllReservationsAsync(CancellationToken ct = default);
    }
}
