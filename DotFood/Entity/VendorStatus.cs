using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DotFood.Entity
{
    public class VendorStatus
    {
        [Key]
        public long Id { get; set; }

        [Required]
        [StringLength(120)]
        public string Status { get; set; }

        [ForeignKey("User")]
        public string UserId { get; set; }
        public virtual Users User { get; set; }
    }
}
