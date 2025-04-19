using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DotFood.Entity
{
    public class Cart
    {
        [Key]
        public long Id { get; set; }

        [ForeignKey("Customer")]
        public string CustomerId { get; set; }
        public Users Customer { get; set; }

        [ForeignKey("Product")]
        public long ProductId { get; set; }
        public Product Product { get; set; }
    }
}
