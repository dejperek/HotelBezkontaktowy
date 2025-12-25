using HotelBezkontaktowy.Models.ViewModels;
using HotelBezkontaktowy.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HotelBezkontaktowy.Controllers
{
    public class RoomsController : Controller
    {
        private readonly IRoomService _roomService;

        public RoomsController(IRoomService roomService)
        {
            _roomService = roomService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(RoomSearchVM filter, CancellationToken ct)
        {
            var roomTypes = await _roomService.GetRoomTypesAsync(ct);
            ViewBag.RoomTypes = new SelectList(roomTypes, "Id", "Name");

            filter.Results = await _roomService.SearchAsync(filter, ct);
            return View(filter);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Book(int id, CancellationToken ct)
        {
            var room = await _roomService.GetByIdAsync(id, ct);
            if (room == null || !room.IsActive)
                return NotFound();

            var vm = new ReservationCreateVM
            {
                RoomId = id,
                DateFrom = DateTime.Today.AddDays(1),
                DateTo = DateTime.Today.AddDays(2),
                GuestsCount = 1
            };

            ViewBag.Room = room;
            return View(vm);
        }
    }
}
