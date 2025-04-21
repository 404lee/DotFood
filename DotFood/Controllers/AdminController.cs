using DotFood.Data;
using DotFood.Entity;
using DotFood.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DotFood.ViewModel;


namespace DotFood.Controllers
{
    [Authorize(Roles = "admin")]
    public class AdminController : Controller
    {
        private readonly UsersContext _context;
        private readonly UserManager<Users> _userManager;

        public AdminController(UsersContext context, UserManager<Users> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users.ToListAsync();
            return View(users);
        }

        public async Task<IActionResult> Analytics()
        {
            var model = new UserManagementViewModel
            {
                TotalRevenue = await _context.Orders.SumAsync(o => o.TotalPrice),
                TotalVendors = (await _userManager.GetUsersInRoleAsync("vendor")).Count, 
                TotalOrders = await _context.Orders.CountAsync(),
            };
            return View(model);
        }

        //[HttpPost]
        //[Authorize(Roles = "vendor,admin")]
        //public async Task<IActionResult> UpdateOrderStatus(long orderId, string status)
        //{
        //    var order = await _context.Orders.FindAsync(orderId);

        //    if (order == null)
        //        return NotFound();

        //    var newStatus = new OrderStatus
        //    {
        //        OrderId = order.Id,
        //        Status = status,
        //        StatusDate = DateTime.Now
        //    };

        //    _context.OrderStatuses.Add(newStatus);
        //    await _context.SaveChangesAsync();

        //    return RedirectToAction("OrderDetails", new { id = order.Id });
        //}

    }
}
