using Event_Registration_Portal.Data;
using Event_Registration_Portal.Models;
using Event_Registration_Portal.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Event_Registration_Portal.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly EventDbContext _context;

        public AdminController(EventDbContext context)
        {
            _context = context;
        }

        // READ: list all events
        public async Task<IActionResult> ManageEvents()
        {
            var events = await _context.Events
                .Include(e => e.Bookings)
                .OrderBy(e => e.EventDate)
                .ToListAsync();

            return View(events);
        }

        // CREATE: show empty form
        [HttpGet]
        public IActionResult CreateEvent()
        {
            return View();
        }

        // CREATE: save new event
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateEvent(CreateEventViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var newEvent = new Event
            {
                Title = model.Title,
                Description = model.Description ?? string.Empty,
                EventDate = model.EventDate!.Value,
                Capacity = model.Capacity!.Value
            };

            _context.Events.Add(newEvent);
            await _context.SaveChangesAsync();

            return RedirectToAction("ManageEvents");
        }

        // UPDATE: show form filled with current values
        [HttpGet]
        public async Task<IActionResult> EditEvent(int id)
        {
            var ev = await _context.Events.FindAsync(id);
            if (ev == null)
                return NotFound();

            return View(new EditEventViewModel
            {
                Id = ev.Id,
                Title = ev.Title,
                Description = ev.Description,
                EventDate = ev.EventDate,
                Capacity = ev.Capacity
            });
        }

        // UPDATE: save changes
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditEvent(EditEventViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var ev = await _context.Events
                .Include(e => e.Bookings)
                .FirstOrDefaultAsync(e => e.Id == model.Id);

            if (ev == null)
                return NotFound();

            int highestSeat = ev.Bookings.Any() ? ev.Bookings.Max(b => b.SeatNumber) : 0;
            if (model.Capacity!.Value < highestSeat)
            {
                ModelState.AddModelError("Capacity",
                    $"Seat {highestSeat} is already booked, so capacity can't be lower than {highestSeat}.");
                return View(model);
            }

            ev.Title = model.Title;
            ev.Description = model.Description ?? string.Empty;
            ev.EventDate = model.EventDate!.Value;
            ev.Capacity = model.Capacity.Value;

            await _context.SaveChangesAsync();

            TempData["Success"] = "Event updated.";
            return RedirectToAction("ManageEvents");
        }

        // DELETE
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            var ev = await _context.Events.FindAsync(id);
            if (ev == null)
                return NotFound();

            _context.Events.Remove(ev);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Event deleted.";
            return RedirectToAction("ManageEvents");
        }
    }
}