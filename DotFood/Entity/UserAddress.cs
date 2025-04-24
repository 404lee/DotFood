using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DotFood.Entity
{
    public class UserAddress
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } // FK to the Identity user

        [ForeignKey("UserId")]
        public Users User { get; set; }

        [Required(ErrorMessage = "Country Name is required")]
        [StringLength(10, ErrorMessage = "Country cannot exceed 10 characters")]
        public string? Country { get; set; }

        [Required(ErrorMessage = "City Name is required")]
        [StringLength(10, ErrorMessage = "city cannot exceed 10 characters")]
        public string? City { get; set; }
    }
}
