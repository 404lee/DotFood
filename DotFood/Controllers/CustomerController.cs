using Microsoft.AspNetCore.Mvc;
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

    [Authorize(Roles = "customer")]

    public class CustomerController : Controller
    {
        private readonly UsersContext _context;

        public CustomerController(UsersContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var products = await _context.Products
                .Include(p => p.Category)
                .ToListAsync();

            return View(products);
        }

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
            var userId = User.Identity.Name;
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

            var cartItem = new Cart
            {
                CustomerId = customer.Id,
                ProductId = product.Id
            };

            _context.Cart.Add(cartItem);
            await _context.SaveChangesAsync();

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

            if (customer == null)
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
