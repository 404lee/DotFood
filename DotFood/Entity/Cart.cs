using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DotFood.Entity
{
    public class Cart
    {
        [Key]
        public long Id { get; set; }

        [ForeignKey("CustomerId")]
        public string CustomerId { get; set; }
        public Users Customer { get; set; }

        [ForeignKey("ProductId")]
        public long ProductId { get; set; }
        public Product Product { get; set; }
        [Required]
        public int Quantity { get; set; }
        [Required]
        public double TotalPrice { get; set; }
    }
}
