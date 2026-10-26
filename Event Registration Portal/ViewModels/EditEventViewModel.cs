using System.ComponentModel.DataAnnotations;

namespace Event_Registration_Portal.ViewModels
{
    public class EditEventViewModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Required]
        [Display(Name = "Event Date & Time")]
        [DataType(DataType.DateTime)]
        [FutureDate(ErrorMessage = "Event date must be in the future.")]
        public DateTime? EventDate { get; set; }

        [Required]
        [Range(1, 10, ErrorMessage = "Capacity must be between 1 and 10.")]
        public int? Capacity { get; set; }
    }
}