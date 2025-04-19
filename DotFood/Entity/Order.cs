using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DotFood.Entity
{
    public class Order
    {
        [Key]
        public long Id { get; set; }

        [ForeignKey("Customer")]
        public String CustomerId { get; set; }

        [ForeignKey("Vendor")]
        public string VendorId { get; set; }

        public DateTime OrderDate { get; set; }

        [DataType(DataType.Currency)]
        public decimal TotalPrice { get; set; }

        public string PaymentMethod { get; set; } = "Cash"; 
        public decimal DeliveryFee { get; set; } = 3.0m; // Fixed 3 JDs

        public virtual ICollection<OrderDetails> OrderDetails { get; set; }

        public virtual Users Customer { get; set; }
        public virtual Users Vendor { get; set; }
    }
}
