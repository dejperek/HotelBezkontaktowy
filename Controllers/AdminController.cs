using HotelBezkontaktowy.Models.Entities;
using HotelBezkontaktowy.Models.ViewModels;
using HotelBezkontaktowy.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HotelBezkontaktowy.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IRoomService _roomService;
        private readonly IReservationService _reservationService;
        private readonly IReportsService _reportsService;

        public AdminController(IRoomService roomService, IReservationService reservationService, IReportsService reportsService)
        {
            _roomService = roomService;
            _reservationService = reservationService;
            _reportsService = reportsService;
        }

        [HttpGet]
        public async Task<IActionResult> Dashboard(DateTime? from, DateTime? to, CancellationToken ct)
        {
            var dashboard = await _reportsService.GetDashboardAsync(from, to, ct);
            return View(dashboard);
        }

        [HttpGet]
        public async Task<IActionResult> Rooms(CancellationToken ct)
        {
            var rooms = await _roomService.GetAllRoomsAsync(ct);
            return View(rooms);
        }

        [HttpGet]
        public async Task<IActionResult> CreateRoom(CancellationToken ct)
        {
            var roomTypes = await _roomService.GetRoomTypesAsync(ct);
            ViewBag.RoomTypes = new SelectList(roomTypes, "Id", "Name");
            return View(new Room());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateRoom(Room room, CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
                var roomTypes = await _roomService.GetRoomTypesAsync(ct);
                ViewBag.RoomTypes = new SelectList(roomTypes, "Id", "Name");
                return View(room);
            }

            await _roomService.CreateRoomAsync(room, ct);
            TempData["Success"] = "Pokój zosta³ dodany.";
            return RedirectToAction(nameof(Rooms));
        }

        [HttpGet]
        public async Task<IActionResult> EditRoom(int id, CancellationToken ct)
        {
            var room = await _roomService.GetByIdAsync(id, ct);
            if (room == null) return NotFound();

            var roomTypes = await _roomService.GetRoomTypesAsync(ct);
            ViewBag.RoomTypes = new SelectList(roomTypes, "Id", "Name");
            return View(room);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditRoom(Room room, CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
                var roomTypes = await _roomService.GetRoomTypesAsync(ct);
                ViewBag.RoomTypes = new SelectList(roomTypes, "Id", "Name");
                return View(room);
            }

            await _roomService.UpdateRoomAsync(room, ct);
            TempData["Success"] = "Pokój zosta³ zaktualizowany.";
            return RedirectToAction(nameof(Rooms));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteRoom(int id, CancellationToken ct)
        {
            await _roomService.DeleteRoomAsync(id, ct);
            TempData["Success"] = "Pokój zosta³ usuniêty.";
            return RedirectToAction(nameof(Rooms));
        }

        [HttpGet]
        public IActionResult CreateRoomType()
        {
            return View(new RoomType());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateRoomType(RoomType roomType, CancellationToken ct)
        {
            if (!ModelState.IsValid)
                return View(roomType);

            await _roomService.CreateRoomTypeAsync(roomType, ct);
            TempData["Success"] = "Typ pokoju zosta³ dodany.";
            return RedirectToAction(nameof(Rooms));
        }

        [HttpGet]
        public async Task<IActionResult> Reservations(CancellationToken ct)
        {
            var reservations = await _reservationService.GetAllReservationsAsync(ct);
            return View(reservations);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeStatus(int id, ReservationStatus status, CancellationToken ct)
        {
            await _reservationService.ChangeStatusAsync(id, status, ct);
            TempData["Success"] = "Status rezerwacji zosta³ zmieniony.";
            return RedirectToAction(nameof(Reservations));
        }
    }
}
