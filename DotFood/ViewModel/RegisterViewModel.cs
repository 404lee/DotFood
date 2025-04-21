using System.ComponentModel.DataAnnotations;

namespace DotFood.ViewModel
{
    public class RegisterViewModel : UserAddressViewModel
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Username is required")]
        [StringLength(20, MinimumLength = 8, ErrorMessage = "Username must be between {2} and {1} characters long")]
        [RegularExpression(@"^[A-Za-z][A-Za-z0-9]{7,19}$",
            ErrorMessage = "Username must be 8 characters, start with a letter and contain only letters and digits (no special characters).")]
        public String FullName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress]
        public String Email { get; set; }


        [Required(ErrorMessage = "Role is required")]
        [Display(Name = "Register As")]
        [RegularExpression(@"^(customer|vendor|admin)$", ErrorMessage = "Role must be one of the following: customer or vendor.")]
        public string Role { get; set; } = "customer";  



        [Required(ErrorMessage = "Password is required")]
        [StringLength(16, MinimumLength = 8, ErrorMessage = "The {0} must be between {2} and {1} characters long")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^A-Za-z0-9]).{8,16}$",
            ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, one digit, and one special character.")]
        [DataType(DataType.Password)]
        public String Password { get; set; }


        [Compare("Password", ErrorMessage = "Password doesn't match")]
        [Required(ErrorMessage = "Confirm Password is required")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        public String ConfirmPassword { get; set; }

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




    }
}
