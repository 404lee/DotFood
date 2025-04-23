//using System.ComponentModel.DataAnnotations;

//namespace DotFood.ViewModel
//{
//    public class ChangePasswordViewModel
//    {
//        [Required(ErrorMessage = "Current password is required")]
//        [DataType(DataType.Password)]
//        [Display(Name = "Current Password")]
//        public string CurrentPassword { get; set; }

//        [Required(ErrorMessage = "Password is required")]
//        [StringLength(16, MinimumLength = 8, ErrorMessage = "The {0} must be between {2} and {1} characters long")]
//        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^A-Za-z0-9]).{8,16}$",
//            ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, one digit, and one special character.")]
//        [DataType(DataType.Password)]
//        public string NewPassword { get; set; }

//        [Required(ErrorMessage = "Please confirm your new password")]
//        [DataType(DataType.Password)]
//        [Display(Name = "Confirm New Password")]
//        [Compare("NewPassword", ErrorMessage = "Passwords do not match")]
//        public string ConfirmPassword { get; set; }
//    }
//}
