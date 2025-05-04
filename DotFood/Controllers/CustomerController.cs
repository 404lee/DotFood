/*using Microsoft.AspNetCore.Mvc;
using DotFood.Data;
using DotFood.Entity;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;


namespace DotFood.Controllers
{

    [Authorize(Roles = "Customer")]

    public class CustomerController : Controller
    {
        private readonly UsersContext _context;

        public CustomerController(UsersContext context)
        {
            _context = context;
        }*/
using Microsoft.AspNetCore.Mvc;
using DotFood.Data;
using DotFood.Entity;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Authorization;
using DotFood.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace DotFood.Controllers
{
    [Authorize( Roles = "Customer")]
    public class CustomerController : Controller
    {
        private readonly UsersContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<Users> _userManager;


        public CustomerController(UsersContext context,RoleManager<IdentityRole> roleManager, UserManager<Users> userManager)
        {
            _context = context;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            //var products = await _context.Products
            //    .Include(p => p.Category)
            //    .ToListAsync();

            var vendors = await _userManager.GetUsersInRoleAsync("vendor");

            

            return View(vendors);
        }
        [HttpGet]
        public async Task<IActionResult> ViewItems(string Id) 
        {
            var categories = await _context.Category.ToListAsync();

            var productViewModel = new ProductViewModel
            {
                Categories = categories,
                VendorId = Id,
            };
            return View(productViewModel);
        }
        
        [HttpGet]
        public async Task<IActionResult> ViewProducts(string vendorId, long categoryId)
        {
            var products = await _context.Products
                .Where(p => p.VendorId == vendorId && p.CategoryId == categoryId)
                .ToListAsync();

            return View(products);
        }

        [HttpGet]
        public async Task<IActionResult> ViewCart()
        {

            var userId = User.Identity.Name;
            var customer = await _context.Users
                .FirstOrDefaultAsync(u => u.UserName == userId);

            if (customer == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var cartItems = await _context.Cart
                .Where(c => c.CustomerId == customer.Id)
                .Include(c => c.Product)
                .ToListAsync();

            return View(cartItems);
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(long productId)
        {
            var userId = User.Identity.Name; //currentlly loged-in user
            var customer = await _context.Users
                .FirstOrDefaultAsync(u => u.UserName == userId);

            if (customer == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.Id == productId);

            if (product == null)
            {
                return RedirectToAction("Index");
            }

            var existingCartItem = await _context.Cart
                .FirstOrDefaultAsync(c => c.CustomerId == customer.Id && c.ProductId == product.Id);

            if (existingCartItem == null)
            {
                var cartItem = new Cart
                {
                    CustomerId = customer.Id,
                    ProductId = product.Id
                };
                _context.Cart.Add(cartItem);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("ViewCart");
        }

        [HttpPost]
        public async Task<IActionResult> RemoveFromCart(long cartId)
        {
            var cartItem = await _context.Cart
                .FirstOrDefaultAsync(c => c.Id == cartId);

            if (cartItem != null)
            {
                _context.Cart.Remove(cartItem);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("ViewCart");
        }

        [HttpPost]
        public async Task<IActionResult> PlaceOrder()
        {
            var userId = User.Identity.Name;
            var customer = await _context.Users
                .FirstOrDefaultAsync(u => u.UserName == userId);

            if (customer == null) // no loged-in user currentlly
            {
                return RedirectToAction("Index", "Home");
            }

            var cartItems = await _context.Cart
                .Where(c => c.CustomerId == customer.Id)
                .Include(c => c.Product)
                .ToListAsync();

            if (!cartItems.Any())
            {
                return RedirectToAction("ViewCart");
            }

            var order = new Order
            {
                CustomerId = customer.Id,
                VendorId = cartItems.First().Product.VendorId,
                OrderDate = DateTime.Now,
                TotalPrice = cartItems.Sum(c => c.Product.Price) + 3.0m, 
                PaymentMethod = "Cash",
                DeliveryFee = 3.0m
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            foreach (var cartItem in cartItems)
            {
                var orderDetails = new OrderDetails
                {
                    OrderId = order.Id,
                    ProductId = cartItem.ProductId,
                    Id = cartItem.Id,
                    Quantity = 1,
                    Price = cartItem.Product.Price
                };

                _context.OrderDetails.Add(orderDetails);
            }

            _context.Cart.RemoveRange(cartItems);
            await _context.SaveChangesAsync();

            return RedirectToAction("OrderConfirmation", new { orderId = order.Id });
        }

        public async Task<IActionResult> OrderConfirmation(long orderId)
        {
            var order = await _context.Orders
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
            {
                return RedirectToAction("Index");
            }

            return View(order);
        }
    }
}
