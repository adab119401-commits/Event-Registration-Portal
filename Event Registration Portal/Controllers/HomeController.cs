using Event_Registration_Portal.Data;
using Event_Registration_Portal.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace Event_Registration_Portal.Controllers
{
    public class HomeController : Controller
    {
        private readonly EventDbContext _context;

        public HomeController(EventDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var upcoming = await _context.Events
                .Include(e => e.Bookings)
                .Where(e => e.EventDate > DateTime.Now)
                .OrderBy(e => e.EventDate)
                .Take(3)
                .ToListAsync();

            return View(upcoming);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}