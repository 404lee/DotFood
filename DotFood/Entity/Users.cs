using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
namespace DotFood.Entity


{
    public class Users : IdentityUser
    {
        public string FullName { get; set; }
        public string Country { get; set; }
        public string City { get; set; }

        public UserAddress Address { get; set; }
        public virtual VendorStatus VendorStatus { get; set; }
        public virtual ICollection<Product> Products { get; set; }
        public virtual ICollection<Order> CustomerOrders { get; set; } 
        public virtual ICollection<Order> VendorOrders { get; set; }       

        
    }
}
