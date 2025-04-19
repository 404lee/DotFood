using DotFood.Data;
using DotFood.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; 

namespace DotFood.Controllers
{
        [Authorize(Roles = "vendor")]
        public class VendorController : Controller
        {
            private readonly UsersContext _context;
            private readonly UserManager<Users> _userManager;

            public VendorController(UsersContext context, UserManager<Users> userManager)
            {
                _context = context;
                _userManager = userManager;
            }

            public async Task<IActionResult> Index()
            {
                var vendor = await _userManager.GetUserAsync(User);
                var products = await _context.Products
                    .Where(p => p.VendorId == vendor.Id)
                    .ToListAsync();

                return View(products);
            }

            [HttpGet]
            public IActionResult AddItem()
            {
                return View();
            }

        [HttpPost]
        public async Task<IActionResult> AddItem(Product model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var vendor = await _userManager.GetUserAsync(User);
            model.VendorId = vendor.Id;
            model.CreatedAt = DateTime.UtcNow;

            _context.Products.Add(model);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }
    }
    
}
