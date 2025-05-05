using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DotFood.Entity
{
    public class Category
    {
        [Key]
        public long Id { get; set; }

        [Required]
        public string Name { get; set; }

        public ICollection<Product> Products { get; set; }


    }
}
