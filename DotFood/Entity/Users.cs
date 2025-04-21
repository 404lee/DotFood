using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
namespace DotFood.Entity


{
    public class Users : IdentityUser
    {
        public string FullName { get; set; }
        public string Country { get; set; }
        public string City { get; set; }

        public virtual VendorStatus VendorStatus { get; set; }
        public virtual ICollection<Product> Products { get; set; }
        public virtual ICollection<Order> Orders { get; set; }

    }
}
