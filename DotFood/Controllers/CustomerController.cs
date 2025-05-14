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
using DotFood.Migrations;
using System.Reflection.Metadata.Ecma335;

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

            var vendors = await _userManager.GetUsersInRoleAsync("vendor");

            foreach (var vendor in vendors)
            {
                var vendorstatus = await _context.VendorStatus.Where(v => v.UserId == vendor.Id).FirstOrDefaultAsync();

                vendor.VendorStatus = vendorstatus;
            }
            await _context.SaveChangesAsync();

            return View(vendors);
        }

        [HttpGet]
        public async Task<IActionResult> ViewCategories(string Id) 
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
            if (TempData["VendorError"] != null)
            {
                ViewBag.VendorError = TempData["VendorError"].ToString();
            }
            
            var CartProducts = await _context.Products
            .Where(p => p.VendorId == vendorId && p.CategoryId == categoryId).ToListAsync();

            var addedProductIds = new List<long>();

            foreach (var product in CartProducts)
            {
                if (product != null)
                {
                    var cartItem = await _context.Cart
                        .FirstOrDefaultAsync(c => c.ProductId == product.Id);

                    if (cartItem != null)
                    {
                        addedProductIds.Add(product.Id);
                    }
                }
            }

            ViewBag.AddingToCart = addedProductIds;

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
        public async Task<IActionResult> AddToCart(long productId,int quantity,double totalPrice)
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

            var ProductVendorId = product?.VendorId;


            if (product == null)
            {
                return RedirectToAction("Index");
            }

            if (quantity < 0)
            {
                return View();
            }

            var cart = await _context.Cart.FirstOrDefaultAsync(c => c.CustomerId == customer.Id);

            var cartItemm = await _context.Cart
                .Where(cart => cart.CustomerId == customer.Id)
                .Include(cart => cart.Product)
                .FirstOrDefaultAsync();

            var currentVendorId = cartItemm?.Product.VendorId;
            
            if (cart == null)
            {
                var existingCartItem = await _context.Cart
                  .FirstOrDefaultAsync(c => c.CustomerId == customer.Id && c.ProductId == product.Id);

                if (existingCartItem == null)
                {
                    var cartItem = new Cart
                    {
                        CustomerId = customer.Id,
                        ProductId = product.Id,
                        Quantity = quantity > 0 ? quantity : 1,
                        TotalPrice = totalPrice
                    };
                    _context.Cart.Add(cartItem);
                    await _context.SaveChangesAsync();
                 
                }
            } 
            else if (product.VendorId == cartItemm?.Product.VendorId)
            {
                var existingCartItem = await _context.Cart
               .FirstOrDefaultAsync(c => c.CustomerId == customer.Id && c.ProductId == product.Id);

                if (existingCartItem == null)
                {
                    var cartItem = new Cart
                    {
                        CustomerId = customer.Id,
                        ProductId = product.Id,
                        Quantity = quantity > 0 ? quantity : 1,
                        TotalPrice = totalPrice
                    };
                    _context.Cart.Add(cartItem);
                    await _context.SaveChangesAsync();
              
                }
            }
            else
            {
                if (ProductVendorId != currentVendorId)
                {
                    TempData["VendorError"] = "You cannot add products from a different vendor in the same cart .";
                    return RedirectToAction("ViewProducts", 
                        new { vendorId = ProductVendorId, categoryId = product.CategoryId});
                }

            }
            return RedirectToAction("ViewProducts",
                        new { vendorId = ProductVendorId, categoryId = product.CategoryId });
        }

        [HttpPost]
        public async Task<IActionResult> RemoveFromCart(long productId)
        {
            var userId = User.Identity.Name;
            var customer = await _context.Users
                .FirstOrDefaultAsync(u => u.UserName == userId);

            if (customer == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var cartItem = await _context.Cart
                .FirstOrDefaultAsync(c => c.CustomerId == customer.Id && c.ProductId == productId);
                
            if (cartItem != null)
            {
                _context.Cart.Remove(cartItem);
                await _context.SaveChangesAsync();
            }

            var remainingCartItems = await _context.Cart
            .Where(c => c.CustomerId == customer.Id)
            .ToListAsync();

            if (!remainingCartItems.Any())
            {
                
                TempData["CartEmptyMessage"] = "Your cart is now empty.";
                return RedirectToAction("ViewCart"); 
            }

            return RedirectToAction("ViewCart");
        }

        [HttpPost]
        public async Task<IActionResult> PlaceOrder(List<Cart> cartItems)
        {
            var userId = User.Identity.Name;
            var customer = await _context.Users.FirstOrDefaultAsync(u => u.UserName == userId);

            if (customer == null || cartItems == null || !cartItems.Any())
                return RedirectToAction("Index", "Home");

            decimal totalPrice = 0;
            var productForVendor = await _context.Products.FindAsync(cartItems.First().ProductId);
            if (productForVendor == null)
                return RedirectToAction("ViewCart");

            foreach (var item in cartItems)
            {
                var product = await _context.Products.FindAsync(item.ProductId);
                if (product == null || item.Quantity > product.Quantity || item.Quantity < 1)
                    return RedirectToAction("ViewCart");

                totalPrice += product.Price * item.Quantity;
            }
            
            
            var order = new Order
            {
                CustomerId = customer.Id,
                VendorId = productForVendor.VendorId,
                OrderDate = DateTime.Now,
                TotalPrice = totalPrice + 3.0m,
                PaymentMethod = "Cash",
                DeliveryFee = 3.0m,
                OrderStatusId = 1, 
                OrderDetails = new List<OrderDetails>() 
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            var orderStatus = new OrderStatus
            {
                OrderId = order.Id,
                Status = OrderState.Approved
            };
            _context.OrderStatus.Add(orderStatus);
            await _context.SaveChangesAsync();

            foreach (var cartItem in cartItems)
            {
                var product = await _context.Products.FindAsync(cartItem.ProductId);

                order.OrderDetails.Add(new OrderDetails
                {
                    ProductId = product.Id,
                    Quantity = cartItem.Quantity,
                    Price = product.Price
                });

                product.Quantity -= cartItem.Quantity; 
            }

            _context.Cart.RemoveRange(cartItems);

            await _context.SaveChangesAsync(); 

            return RedirectToAction("OrderConfirmation", new { orderId = order.Id });
        }


        [HttpGet]
        public async Task<IActionResult> OrderConfirmation(long orderId)
        {
            var order = await _context.Orders
                .Include(o => o.OrderStatus)
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
