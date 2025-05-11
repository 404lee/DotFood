using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DotFood.Entity
{
    public class Order
    {
        [Key]
        public long Id { get; set; }

        [ForeignKey("CustomerId")]
        public String CustomerId { get; set; }

        [ForeignKey("VendorId")]
        public string VendorId { get; set; }

        public DateTime OrderDate { get; set; }

        [DataType(DataType.Currency)]
        public decimal TotalPrice { get; set; }

        public string PaymentMethod { get; set; } = "Cash"; 
        public decimal DeliveryFee { get; set; } = 3.0m; // Fixed 3 JDs

        public virtual ICollection<OrderDetails> OrderDetails { get; set; } = new List<OrderDetails>();

        public  Users Customer { get; set; }
        public  Users Vendor { get; set; }

        public int OrderStatusId { get; set; }
        public OrderStatus OrderStatus { get; set; }

    }
}
