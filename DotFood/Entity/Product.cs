using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DotFood.Entity
{
    public class Product
    {
        [Key]
        public long Id { get; set; }

        [Required]
        [ForeignKey("CategoryId")]
        public long CategoryId { get; set; }
        public virtual Category Category { get; set; }

        [Required]
        [ForeignKey("VendorId")]
        public string VendorId { get; set; }
        public virtual Users Vendor { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Price { get; set; }

        [Required]
        [Range(0, long.MaxValue)]
        public long Quantity { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public bool IsAvailable { get; set; } = true;


    }
}