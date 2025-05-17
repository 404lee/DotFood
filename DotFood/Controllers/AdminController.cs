using DotFood.Data;
using DotFood.Entity;
using DotFood.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace DotFood.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly UsersContext _context;
        private readonly UserManager<Users> _userManager;

        public AdminController(UsersContext context, UserManager<Users> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users.ToListAsync();
            return View(users);
        }
        
        [HttpGet]
        public async Task<IActionResult> Analytics()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> VendorAnalytics()
        {
            var AnalyticsPerVendor = await _context.Orders
                .GroupBy(o => o.VendorId)
                .Select(g => new
                {
                    VendorId = g.Key,
                    VendorName = g.FirstOrDefault().Vendor.FullName,
                    TotalOrders = g.Count(),
                    TotalRevenue = g.Sum(o => o.TotalPrice),
                })
                .OrderByDescending(v => v.TotalRevenue)
                .ToListAsync();

            var analyticsList = new List<AnalyticsViewModelForEachVendor>();

            foreach (var item in AnalyticsPerVendor)
            {
                var vendorUser = await _userManager.FindByIdAsync(item.VendorId);


                analyticsList.Add(new AnalyticsViewModelForEachVendor
                {
                    VendorId = vendorUser,
                    VendorName = item.VendorName,
                    TotalOrders = item.TotalOrders,
                    TotalRevenue = item.TotalRevenue
                });

            }

            var model = new VendorAnalyticsViewModel
            {
                analyticsViewModelForEachVendor = analyticsList,
                TotalRevenue = await _context.Orders.SumAsync(o => o.TotalPrice),
                TotalVendors = (await _userManager.GetUsersInRoleAsync("Vendor")).Count,
                TotalOrders = await _context.Orders.CountAsync(),
            };

            return View(model);
        }


        [HttpGet]
        public async Task<IActionResult> CustomerAnalytics()
        {
            var AnalyticsPerCutomer = await _context.Orders
                .GroupBy(c => c.CustomerId)
                .Select(g => new
                {
                    CutsomerId = g.Key,
                    CustomerName = g.FirstOrDefault().Customer.FullName,
                    TotalOrders = g.Count(),
                    TotalRevenue = g.Sum(g => g.TotalPrice),
                })
                .OrderByDescending(g => g.TotalRevenue)
                .ToListAsync();

            var analyticsList = new List<AnalyticsForEachCustomers>();

            foreach (var item in AnalyticsPerCutomer)
            {
                var customerUser = await _userManager.FindByIdAsync(item.CutsomerId);

                analyticsList.Add(new AnalyticsForEachCustomers
                {
                    CustomerId = customerUser,
                    CustomerName = item.CustomerName,
                    TotalOrders = item.TotalOrders,
                    TotalRevenue = item.TotalRevenue
                });
            }

            var model = new CustomerAnalyticsViewModel
            {
                analyticsForEachCustomers = analyticsList,
                TotalRevenue = await _context.Orders.SumAsync(o => o.TotalPrice),
                TotalCustomers = (await _userManager.GetUsersInRoleAsync("Customer")).Count,
                TotalOrders = await _context.Orders.CountAsync(),
            };
            return View(model);

        }


        [HttpGet]
        public async Task<IActionResult> ManageUsers()
        {
            var users = await _userManager.Users.ToListAsync();

            var usersWithRoles = new List<UserWithRolesViewModel>();

            foreach (var user in users)
            {
                var roles = (await _userManager.GetRolesAsync(user)).ToList();
                usersWithRoles.Add(new UserWithRolesViewModel
                {
                    User = user,
                    Roles = roles
                });
            }

            var viewModel = new ManageUsersViewModel
            {
                UsersWithRoles = usersWithRoles
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            var roles = await _userManager.GetRolesAsync(user);

            if (roles.Contains("Vendor"))
            {
                var vendorOrders = await _context.Orders.Where(o => o.VendorId == id).ToListAsync();
                foreach (var order in vendorOrders)
                {   
                    var orderItems = await _context.OrderDetails.Where(oi => oi.OrderId == order.Id).ToListAsync();
                    _context.OrderDetails.RemoveRange(orderItems);
                    _context.Orders.Remove(order);
                }
                var products = await _context.Products.Where(p => p.VendorId == id).ToListAsync();
                foreach (var product in products)
                {
                    var cartItems = await _context.Cart.Where(c => c.ProductId == product.Id).ToListAsync();
                    _context.Cart.RemoveRange(cartItems);

                    var productOrderDetails = await _context.OrderDetails.Where(od => od.ProductId == product.Id).ToListAsync();
                    _context.OrderDetails.RemoveRange(productOrderDetails);
                    _context.Products.Remove(product);
                }
            }
            else if (roles.Contains("Customer"))
            {
                var customerOrders = await _context.Orders.Where(o => o.CustomerId == id).ToListAsync();
                foreach (var order in customerOrders)
                {
                    var orderItems = await _context.OrderDetails.Where(oi => oi.OrderId == order.Id).ToListAsync();
                    _context.OrderDetails.RemoveRange(orderItems);
                    _context.Orders.Remove(order);
                }
                var cartItems = await _context.Cart.Where(c => c.CustomerId == id).ToListAsync();
                _context.Cart.RemoveRange(cartItems);

            }
            await _context.SaveChangesAsync();
            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "User deleted successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "There was an error deleting the user.";
            }
            return RedirectToAction("ManageUsers");
        }

        [HttpGet]
        public async Task<IActionResult> ViewHistory(string id)
        {
            //var users = await _userManager.Users.ToListAsync();
            var user = await _userManager.FindByIdAsync(id);
            var userHistory = new List<UserHistory>();

            //foreach (var user in users)
            //{
                var roles = await _userManager.GetRolesAsync(user);

                if (roles.Contains("Vendor"))
                {
                    var vendorOrders = await _context.Orders
                        .Where(o => o.VendorId == id)
                        .Select(o => new OrderHistory
                        {
                            OrderId = o.Id,
                            CustomerName = o.Customer.FullName,
                            OrderDate = o.OrderDate,
                            TotalPrice = o.TotalPrice
                        })
                        .ToListAsync();

                    userHistory.Add(new UserHistory
                    {
                        UserId = user.Id,
                        FullName = user.FullName,
                        Role = "Vendor",
                        Orders = vendorOrders
                    });
                }
                else if (roles.Contains("Customer"))
                {
                    var customerOrders = await _context.Orders
                        .Where(o => o.CustomerId == user.Id)
                        .Select(o => new OrderHistory
                        {
                            OrderId = o.Id,
                            VendorName = o.Vendor.FullName,
                            OrderDate = o.OrderDate,
                            TotalPrice = o.TotalPrice
                        })
                        .ToListAsync();

                    userHistory.Add(new UserHistory
                    {
                        UserId = user.Id,
                        FullName = user.FullName,
                        Role = "Customer",
                        Orders = customerOrders
                    });
                }
            //}
            return View(userHistory);
        }

    }
}
