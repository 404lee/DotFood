using System.ComponentModel.DataAnnotations;

namespace DotFood.ViewModel
{
    public class EditProfileViewModel
    {
        [Required(ErrorMessage = "Username is required")]
        [StringLength(20, MinimumLength = 8, ErrorMessage = "Username must be between {2} and {1} characters long")]
        [RegularExpression(@"^[A-Za-z][A-Za-z0-9]{7,19}$",
            ErrorMessage = "Username must start with a letter and contain only letters and digits (no special characters).")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Country Name is required")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Country name must be between {2} and {1} characters long")]
        [RegularExpression(@"^[A-Za-z][A-Za-z\s]{2,49}$",
            ErrorMessage = "Country name must start with a letter and contain only letters and spaces (no digits or special characters).")]
        public string Country { get; set; }


        [Required(ErrorMessage = "City Name is required")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "City name must be between {2} and {1} characters long")]
        [RegularExpression(@"^[A-Za-z][A-Za-z\s]{2,49}$",
            ErrorMessage = "City name must start with a letter and contain only letters and spaces (no digits or special characters).")]
        public string City { get; set; }

        public ChangeEmailViewModel EmailModel { get; set; } = new();
        public ChangePasswordViewModel PasswordModel { get; set; } = new();
    }
}
