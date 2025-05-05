using DotFood.Entity;
using System.ComponentModel.DataAnnotations;

namespace DotFood.ViewModel
{
    public class ProductViewModel2
    {

        [Required]
        [Display(Name = "Product Name")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Category")]
        public long CategoryId { get; set; }
        //public Category Category { get; internal set; }

        public List<DotFood.Entity.Category> Categories { get; set; } = new();

        [StringLength(500)]
        public string Description { get; set; }

        [Required]
        [Range(0.01, 99999)]    
        public decimal Price { get; set; }

        [Required]
        [Range(1, 100000)]
        public long Quantity { get; set; }

        public IFormFile? ImageFile { get; set; }  // For new uploads

        public string? imageName { get; set; }
    }
}
