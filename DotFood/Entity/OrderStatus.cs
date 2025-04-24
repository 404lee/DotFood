using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using DotFood.Entity;

namespace DotFood.Entity
{
    public enum OrderState
    {
        Pending,
        Approved,
        Cancelled
    }

    public class OrderStatus
    {
        [Key]
        public long Id { get; set; }

        [Required]
        public OrderState Status { get; set; } 

        [ForeignKey("OrderId")]
        public long? OrderId { get; set; }
        public virtual Order Order { get; set; }
    }
}
