using System.ComponentModel.DataAnnotations;

namespace DotFood.ViewModel
{
    public class ChangeEmailViewModel
    {

        [Required]
        [EmailAddress]
        [Display(Name = "New Email")]
        public string NewEmail { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current Password")]
        public string CurrentPassword { get; set; }
    }
}
