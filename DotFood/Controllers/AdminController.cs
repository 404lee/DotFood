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

        public async Task<IActionResult> ManageUsers()
        {
            var users = await _userManager.Users.ToListAsync();
            return View(users);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();

            await _userManager.DeleteAsync(user);
            return RedirectToAction(nameof(ManageUsers));
        }

    }
}
