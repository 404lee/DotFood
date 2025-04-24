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

        public virtual ICollection<OrderDetails> OrderDetails { get; set; }

        public virtual Users Customer { get; set; }
        public virtual Users Vendor { get; set; }

        public int OrderStatusId { get; set; }
        public virtual OrderStatus OrderStatus { get; set; }

    }
}
