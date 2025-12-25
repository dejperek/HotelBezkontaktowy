using HotelBezkontaktowy.Data;
using HotelBezkontaktowy.Models.ViewModels;
using HotelBezkontaktowy.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HotelBezkontaktowy.Controllers
{
    [Authorize]
    public class ReservationsController : Controller
    {
        private readonly IReservationService _reservationService;
        private readonly IRoomService _roomService;
        private readonly UserManager<ApplicationUser> _userManager;

        public ReservationsController(IReservationService reservationService, IRoomService roomService, UserManager<ApplicationUser> userManager)
        {
            _reservationService = reservationService;
            _roomService = roomService;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var userId = _userManager.GetUserId(User)!;
            var reservations = await _reservationService.GetUserReservationsAsync(userId, ct);
            return View(reservations);
        }

        [HttpGet]
        public async Task<IActionResult> Create(int roomId, CancellationToken ct)
        {
            var room = await _roomService.GetByIdAsync(roomId, ct);
            if (room == null || !room.IsActive)
                return NotFound();

            var vm = new ReservationCreateVM
            {
                RoomId = roomId,
                DateFrom = DateTime.Today.AddDays(1),
                DateTo = DateTime.Today.AddDays(2),
                GuestsCount = 1
            };

            ViewBag.Room = room;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ReservationCreateVM vm, CancellationToken ct)
        {
            var room = await _roomService.GetByIdAsync(vm.RoomId, ct);
            if (room == null || !room.IsActive)
                return NotFound();

            ViewBag.Room = room;

            if (!ModelState.IsValid)
                return View(vm);

            // Additional server-side validations
            if (vm.GuestsCount > room.Capacity)
            {
                ModelState.AddModelError(nameof(vm.GuestsCount), $"Maksymalna liczba goœci to {room.Capacity}.");
                return View(vm);
            }

            var hasConflict = await _reservationService.HasConflictsAsync(vm.RoomId, vm.DateFrom, vm.DateTo, null, ct);
            if (hasConflict)
            {
                ModelState.AddModelError(string.Empty, "Wybrany termin jest ju¿ zajêty. Proszê wybraæ inny termin.");
                return View(vm);
            }

            try
            {
                var userId = _userManager.GetUserId(User)!;
                var reservationId = await _reservationService.CreateAsync(userId, vm, ct);
                TempData["Success"] = "Rezerwacja zosta³a utworzona pomyœlnie!";
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GenerateToken(int id, CancellationToken ct)
        {
            var userId = _userManager.GetUserId(User)!;
            var token = await _reservationService.GenerateAccessTokenAsync(id, userId, ct);

            if (token == null)
                return NotFound();

            TempData["AccessToken"] = token;
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Access(string token, CancellationToken ct)
        {
            if (string.IsNullOrEmpty(token))
                return View("AccessForm");

            var reservation = await _reservationService.GetByAccessTokenAsync(token, ct);
            if (reservation == null)
            {
                ViewBag.Error = "Nieprawid³owy token dostêpu.";
                return View("AccessForm");
            }

            return View("AccessDetails", reservation);
        }
    }
}
