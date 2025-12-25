using HotelBezkontaktowy.Data;
using HotelBezkontaktowy.Models.Entities;
using HotelBezkontaktowy.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace HotelBezkontaktowy.Services
{
    public class RoomService : IRoomService
    {
        private readonly AppDbContext _db;

        public RoomService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<List<RoomListItemVM>> SearchAsync(RoomSearchVM filter, CancellationToken ct = default)
        {
            var query = _db.Rooms
                .AsNoTracking()
                .Include(r => r.RoomType)
                .Where(r => r.IsActive);

            if (filter.Guests.HasValue)
                query = query.Where(r => r.Capacity >= filter.Guests.Value);

            if (filter.RoomTypeId.HasValue)
                query = query.Where(r => r.RoomTypeId == filter.RoomTypeId.Value);

            if (filter.MinPrice.HasValue)
                query = query.Where(r => r.PricePerNight >= filter.MinPrice.Value);

            if (filter.MaxPrice.HasValue)
                query = query.Where(r => r.PricePerNight <= filter.MaxPrice.Value);

            if (filter.DateFrom.HasValue && filter.DateTo.HasValue)
            {
                var from = filter.DateFrom.Value.Date;
                var to = filter.DateTo.Value.Date;
                query = query.Where(r => !_db.Reservations
                    .Any(res => res.RoomId == r.Id &&
                                res.Status != ReservationStatus.Cancelled && res.Status != ReservationStatus.Completed &&
                                res.DateFrom < to && from < res.DateTo));
            }

            return await query
                .OrderBy(r => (double)r.PricePerNight)
                .Select(r => new RoomListItemVM
                {
                    Id = r.Id,
                    RoomNumber = r.RoomNumber,
                    RoomType = r.RoomType!.Name,
                    Capacity = r.Capacity,
                    PricePerNight = r.PricePerNight,
                    IsActive = r.IsActive
                })
                .ToListAsync(ct);
        }

        public async Task<List<RoomListItemVM>> GetAllRoomsAsync(CancellationToken ct = default)
        {
            return await _db.Rooms
                .AsNoTracking()
                .Include(r => r.RoomType)
                .OrderBy(r => r.RoomNumber)
                .Select(r => new RoomListItemVM
                {
                    Id = r.Id,
                    RoomNumber = r.RoomNumber,
                    RoomType = r.RoomType!.Name,
                    Capacity = r.Capacity,
                    PricePerNight = r.PricePerNight,
                    IsActive = r.IsActive
                })
                .ToListAsync(ct);
        }

        public Task<Room?> GetByIdAsync(int id, CancellationToken ct = default)
            => _db.Rooms.Include(r => r.RoomType).FirstOrDefaultAsync(r => r.Id == id, ct);

        public Task<List<RoomType>> GetRoomTypesAsync(CancellationToken ct = default)
            => _db.RoomTypes.AsNoTracking().OrderBy(t => t.Name).ToListAsync(ct);

        public async Task CreateRoomTypeAsync(RoomType type, CancellationToken ct = default)
        {
            _db.RoomTypes.Add(type);
            await _db.SaveChangesAsync(ct);
        }

        public async Task CreateRoomAsync(Room room, CancellationToken ct = default)
        {
            _db.Rooms.Add(room);
            await _db.SaveChangesAsync(ct);
        }

        public async Task UpdateRoomAsync(Room room, CancellationToken ct = default)
        {
            _db.Rooms.Update(room);
            await _db.SaveChangesAsync(ct);
        }

        public async Task DeleteRoomAsync(int id, CancellationToken ct = default)
        {
            var entity = await _db.Rooms.FindAsync(new object[] { id }, ct);
            if (entity != null)
            {
                _db.Rooms.Remove(entity);
                await _db.SaveChangesAsync(ct);
            }
        }
    }
}
