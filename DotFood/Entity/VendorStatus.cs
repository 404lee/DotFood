using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DotFood.Entity
{
    public enum StoreStatus
    {
        Open,
        Closed
    }
    public class VendorStatus
    {

        [Key]
        public long Id { get; set; }


        [Required]
        public StoreStatus Status { get; set; }  


        [ForeignKey("UserId")]
        public string UserId { get; set; }
        public virtual Users User { get; set; }
    }
}
