using System.ComponentModel.DataAnnotations;

namespace Event_Registration_Portal.Models
{
    public class User
    {
        public int Id { get; set; }
        [Required] 
        public string Name { get; set; } = "";
        [Required]
        public string Email { get; set; } = "";
        [Required]
        public string PasswordHash { get; set; } = "";
        public string Role { get; set; } = "User"; // Default role is "User"
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }
}
