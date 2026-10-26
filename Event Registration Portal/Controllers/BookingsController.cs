using System.Security.Claims;
using Event_Registration_Portal.Data;
using Event_Registration_Portal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Event_Registration_Portal.Controllers
{
    [Authorize]
    public class BookingsController : Controller
    {
        private readonly EventDbContext _context;

        public BookingsController(EventDbContext context)
        {
            _context = context;
        }

        private int GetUserId()
        {
            return int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Book(int eventId)
        {
            int userId = GetUserId();

            var ev = await _context.Events
                .Include(e => e.Bookings)
                .FirstOrDefaultAsync(e => e.Id == eventId);

            if (ev == null)
                return NotFound();

            if (ev.EventDate <= DateTime.Now)
                return Fail("This event has already taken place.");

            if (DateTime.Now > ev.RegistrationDeadLine)
                return Fail("Registration for this event has closed.");

            if (ev.Bookings.Any(b => b.UserId == userId))
                return Fail("You have already booked this event.");

            if (ev.Bookings.Count >= ev.Capacity)
                return Fail("Sorry, this event is fully booked.");

            int seat = Enumerable.Range(1, ev.Capacity)
                .First(s => !ev.Bookings.Any(b => b.SeatNumber == s));

            var booking = new Booking
            {
                UserId = userId,
                EventId = ev.Id,
                SeatNumber = seat
            };

            var waitEntry = await _context.WaitlistEntries
                .FirstOrDefaultAsync(w => w.UserId == userId && w.EventId == eventId);
            if (waitEntry != null)
                _context.WaitlistEntries.Remove(waitEntry);

            try
            {
                _context.Bookings.Add(booking);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return Fail("That seat was just taken. Please try again.");
            }

            return RedirectToAction("Confirmation", new { id = booking.Id });
        }

        public async Task<IActionResult> Confirmation(int id)
        {
            int userId = GetUserId();

            var booking = await _context.Bookings
                .Include(b => b.Event)
                .Include(b => b.User)
                .FirstOrDefaultAsync(b => b.Id == id && b.UserId == userId);

            if (booking == null)
                return NotFound();

            return View(booking);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id)
        {
            int userId = GetUserId();

            var booking = await _context.Bookings
                .Include(b => b.Event)
                .FirstOrDefaultAsync(b => b.Id == id && b.UserId == userId);

            if (booking == null)
                return NotFound();

            if (booking.Event.EventDate <= DateTime.Now)
                return Fail("You can't cancel a booking for a past event.");

            int freedSeat = booking.SeatNumber;
            int cancelledEventId = booking.EventId;

            _context.Bookings.Remove(booking);
            await _context.SaveChangesAsync();

            var next = await _context.WaitlistEntries
                .Where(w => w.EventId == cancelledEventId)
                .OrderBy(w => w.JoinedOn)
                .FirstOrDefaultAsync();

            if (next != null)
            {
                _context.Bookings.Add(new Booking
                {
                    UserId = next.UserId,
                    EventId = cancelledEventId,
                    SeatNumber = freedSeat
                });
                _context.WaitlistEntries.Remove(next);
                await _context.SaveChangesAsync();
            }

            TempData["Success"] = "Booking cancelled. Your seat has been released.";
            return RedirectToAction("Index", "Events");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> JoinWaitlist(int eventId)
        {
            int userId = GetUserId();

            var ev = await _context.Events
                .Include(e => e.Bookings)
                .FirstOrDefaultAsync(e => e.Id == eventId);

            if (ev == null)
                return NotFound();

            if (DateTime.Now > ev.RegistrationDeadLine)
                return Fail("Registration for this event has closed.");

            if (ev.Bookings.Count < ev.Capacity)
                return Fail("Seats are available, you can book directly.");

            if (ev.Bookings.Any(b => b.UserId == userId))
                return Fail("You have already booked this event.");

            bool alreadyWaiting = await _context.WaitlistEntries
                .AnyAsync(w => w.UserId == userId && w.EventId == eventId);
            if (alreadyWaiting)
                return Fail("You are already on the waitlist.");

            _context.WaitlistEntries.Add(new WaitlistEntry { UserId = userId, EventId = eventId });
            await _context.SaveChangesAsync();

            TempData["Success"] = "You're on the waitlist. If a seat opens, you'll be booked automatically.";
            return RedirectToAction("Index", "Events");
        }

        private IActionResult Fail(string message)
        {
            TempData["Error"] = message;
            return RedirectToAction("Index", "Events");
        }
    }
}