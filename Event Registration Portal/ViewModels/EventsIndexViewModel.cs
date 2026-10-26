using Event_Registration_Portal.Models;

namespace Event_Registration_Portal.ViewModels
{
    public class EventsIndexViewModel
    {
        public List<Booking> MyBookings { get; set; } = new List<Booking>();
        public List<Event> AvailableEvents { get; set; } = new List<Event>();
        public List<int> WaitlistEventIds { get; set; } = new List<int>();

    }
}