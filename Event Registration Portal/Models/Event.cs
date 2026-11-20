using System.ComponentModel.DataAnnotations;

namespace Event_Registration_Portal.Models
{
    public class Event
    {
        public int Id { get; set; }
        [Required] 
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        [Required]
        public DateTime EventDate { get; set; }
        [Range(1, 500, ErrorMessage = "Capacity must be between 1 and 500.")]
        public int Capacity { get; set; }
        public DateTime RegistrationDeadLine => EventDate.AddDays(-3); // Registration deadline is 3 days before the event
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }
}
