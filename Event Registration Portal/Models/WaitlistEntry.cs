namespace Event_Registration_Portal.Models
{
    public class WaitlistEntry
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public int EventId { get; set; }
        public Event Event { get; set; } = null!;

        public DateTime JoinedOn { get; set; } = DateTime.Now;
    }
}