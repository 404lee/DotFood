using System.ComponentModel.DataAnnotations;

namespace DotFood.ViewModel
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "email is required")]
        [EmailAddress(ErrorMessage = "Please enter a valid e-mail address")]
        public String Email { get; set; }

        [Required(ErrorMessage = "password is required")]
        [DataType(DataType.Password)]
        public String Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }
}
