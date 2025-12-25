using System.Security.Cryptography;
using System.Text;
using HotelBezkontaktowy.Data;
using HotelBezkontaktowy.Models.Entities;
using HotelBezkontaktowy.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace HotelBezkontaktowy.Services
{
    public class ReservationService : IReservationService
    {
        private readonly AppDbContext _db;

        public ReservationService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<bool> HasConflictsAsync(int roomId, DateTime from, DateTime to, int? excludeReservationId = null, CancellationToken ct = default)
        {
            return await _db.Reservations.AnyAsync(r => r.RoomId == roomId
                && r.Status != ReservationStatus.Cancelled
                && r.Status != ReservationStatus.Completed
                && r.DateFrom < to && from < r.DateTo
                && (!excludeReservationId.HasValue || r.Id != excludeReservationId.Value), ct);
        }

        public async Task<int> CreateAsync(string userId, ReservationCreateVM vm, CancellationToken ct = default)
        {
            var room = await _db.Rooms.FirstOrDefaultAsync(r => r.Id == vm.RoomId, ct);
            if (room == null || !room.IsActive)
                throw new InvalidOperationException("Pokój nie istnieje lub jest nieaktywny.");
            if (vm.GuestsCount > room.Capacity)
                throw new InvalidOperationException("Liczba goœci przekracza pojemnoœæ pokoju.");
            if (await HasConflictsAsync(vm.RoomId, vm.DateFrom.Date, vm.DateTo.Date, null, ct))
                throw new InvalidOperationException("Istnieje konflikt rezerwacji dla tego terminu.");

            var nights = (int)Math.Ceiling((vm.DateTo.Date - vm.DateFrom.Date).TotalDays);
            var total = nights * room.PricePerNight;

            var entity = new Reservation
            {
                UserId = userId,
                RoomId = vm.RoomId,
                DateFrom = vm.DateFrom.Date,
                DateTo = vm.DateTo.Date,
                GuestsCount = vm.GuestsCount,
                Status = ReservationStatus.Pending,
                TotalPrice = total,
                CreatedAt = DateTime.UtcNow
            };

            _db.Reservations.Add(entity);
            await _db.SaveChangesAsync(ct);
            return entity.Id;
        }

        public async Task<List<ReservationListItemVM>> GetUserReservationsAsync(string userId, CancellationToken ct = default)
        {
            return await _db.Reservations
                .AsNoTracking()
                .Where(r => r.UserId == userId)
                .Include(r => r.Room)!.ThenInclude(rm => rm!.RoomType)
                .OrderByDescending(r => r.CreatedAt)
                .Select(r => new ReservationListItemVM
                {
                    Id = r.Id,
                    RoomNumber = r.Room!.RoomNumber,
                    RoomType = r.Room!.RoomType!.Name,
                    DateFrom = r.DateFrom,
                    DateTo = r.DateTo,
                    GuestsCount = r.GuestsCount,
                    Status = r.Status.ToString(),
                    TotalPrice = r.TotalPrice
                })
                .ToListAsync(ct);
        }

        public async Task<string?> GenerateAccessTokenAsync(int reservationId, string userId, CancellationToken ct = default)
        {
            var res = await _db.Reservations.FirstOrDefaultAsync(r => r.Id == reservationId && r.UserId == userId, ct);
            if (res == null) return null;

            var token = Convert.ToHexString(RandomNumberGenerator.GetBytes(16));
            res.AccessToken = token;
            await _db.SaveChangesAsync(ct);
            return token;
        }

        public async Task<bool> ChangeStatusAsync(int reservationId, ReservationStatus status, CancellationToken ct = default)
        {
            var res = await _db.Reservations.FindAsync(new object[] { reservationId }, ct);
            if (res == null) return false;
            res.Status = status;
            await _db.SaveChangesAsync(ct);
            return true;
        }

        public Task<Reservation?> GetByAccessTokenAsync(string token, CancellationToken ct = default)
        {
            return _db.Reservations
                .Include(r => r.Room)!.ThenInclude(rm => rm!.RoomType)
                .FirstOrDefaultAsync(r => r.AccessToken == token, ct);
        }

        public async Task<List<ReservationListItemVM>> GetAllReservationsAsync(CancellationToken ct = default)
        {
            return await _db.Reservations
                .AsNoTracking()
                .Include(r => r.Room)!.ThenInclude(rm => rm!.RoomType)
                .Include(r => r.User)
                .OrderByDescending(r => r.CreatedAt)
                .Select(r => new ReservationListItemVM
                {
                    Id = r.Id,
                    RoomNumber = r.Room!.RoomNumber,
                    RoomType = r.Room!.RoomType!.Name,
                    DateFrom = r.DateFrom,
                    DateTo = r.DateTo,
                    GuestsCount = r.GuestsCount,
                    Status = r.Status.ToString(),
                    TotalPrice = r.TotalPrice
                })
                .ToListAsync(ct);
        }
    }
}
