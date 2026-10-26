using System.Security.Claims;
using Event_Registration_Portal.Data;
using Event_Registration_Portal.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Event_Registration_Portal.Controllers
{
    [Authorize]
    public class EventsController : Controller
    {
        private readonly EventDbContext _context;

        public EventsController(EventDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(DateTime? fromDate, DateTime? toDate)
        {
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var myBookings = await _context.Bookings
                .Include(b => b.Event)
                .Where(b => b.UserId == userId)
                .OrderBy(b => b.Event.EventDate)
                .ToListAsync();

            var bookedEventIds = myBookings.Select(b => b.EventId).ToList();

            var waitlistEventIds = await _context.WaitlistEntries
                .Where(w => w.UserId == userId)
                .Select(w => w.EventId)
                .ToListAsync();

            var query = _context.Events
                .Include(e => e.Bookings)
                .Where(e => e.EventDate > DateTime.Now && !bookedEventIds.Contains(e.Id));

            if (fromDate.HasValue)
                query = query.Where(e => e.EventDate >= fromDate.Value.Date);
            if (toDate.HasValue)
                query = query.Where(e => e.EventDate < toDate.Value.Date.AddDays(1));

            var availableEvents = await query.OrderBy(e => e.EventDate).ToListAsync();

            ViewData["From"] = fromDate?.ToString("yyyy-MM-dd");
            ViewData["To"] = toDate?.ToString("yyyy-MM-dd");

            return View(new EventsIndexViewModel
            {
                MyBookings = myBookings,
                AvailableEvents = availableEvents,
                WaitlistEventIds = waitlistEventIds
            });
        }
    }
}