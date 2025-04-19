using System.ComponentModel.DataAnnotations;

namespace DotFood.ViewModel
{
    public class UserAddressViewModel
    {

        [Required(ErrorMessage = "Country Name is required")]
        [StringLength(10, ErrorMessage = "Country cannot exceed 10 characters")]
        public string? Country { get; set; }

        [Required(ErrorMessage = "City Name is required")]
        [StringLength(10, ErrorMessage = "city cannot exceed 10 characters")]
        public string? City { get; set; }

    } 
}
