using System.ComponentModel.DataAnnotations;

namespace Event_Registration_Portal.ViewModels
{
    public class FutureDateAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is DateTime date && date > DateTime.Now)
                return ValidationResult.Success;

            return new ValidationResult(ErrorMessage ?? "Date must be in the future.");
        }
    }
}