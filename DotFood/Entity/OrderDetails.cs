using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DotFood.Entity
{
    public class OrderDetails
    {
        [Key]
        public long Id { get; set; }

        [ForeignKey("Order")]
        public long OrderId { get; set; }

        [ForeignKey("Product")]
        public long ProductId { get; set; }

        public long Quantity { get; set; }

        [DataType(DataType.Currency)]
        public decimal Price { get; set; }

        public virtual Order Order { get; set; }
        public virtual Product Product { get; set; }
    }
}
