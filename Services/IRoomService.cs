using HotelBezkontaktowy.Models.Entities;
using HotelBezkontaktowy.Models.ViewModels;

namespace HotelBezkontaktowy.Services
{
    public interface IRoomService
    {
        Task<List<RoomListItemVM>> SearchAsync(RoomSearchVM filter, CancellationToken ct = default);
        Task<List<RoomListItemVM>> GetAllRoomsAsync(CancellationToken ct = default);
        Task<Room?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<List<RoomType>> GetRoomTypesAsync(CancellationToken ct = default);
        Task CreateRoomTypeAsync(RoomType type, CancellationToken ct = default);
        Task CreateRoomAsync(Room room, CancellationToken ct = default);
        Task UpdateRoomAsync(Room room, CancellationToken ct = default);
        Task DeleteRoomAsync(int id, CancellationToken ct = default);
    }
}
