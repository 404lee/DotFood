using DotFood.Data;
using DotFood.Entity;
using DotFood.ViewModels;
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
        public async Task<IActionResult> ViewStore()
        {
            var vendor = await _userManager.GetUserAsync(User);
            var products = await _context.Products
                .Where(p => p.VendorId == vendor.Id)
                .ToListAsync();

            return View(products);
        }

        [HttpGet]
        public async Task<IActionResult> AddItem()
        {
            var categories = await _context.Category.ToListAsync();

            var model = new ProductViewModel
            {
                Categories = categories
            };
            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddItem(ProductViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Categories = await _context.Category.ToListAsync();
                return View(model);
            }

            var vendor = await _userManager.GetUserAsync(User);

            bool isDuplicate = await _context.Products
            .AnyAsync(p => p.VendorId == vendor.Id && p.Name.ToLower() == model.Name.ToLower() && p.CategoryId == model.CategoryId);

            if (isDuplicate)
            {
                ModelState.AddModelError("Name", "A product with the same name already exists.");
                model.Categories = await _context.Category.ToListAsync();
                return View(model);
            }
            var product = new Product
            {
                Name = model.Name,
                CategoryId = model.CategoryId,
                Description = model.Description,
                Price = model.Price,
                Quantity = model.Quantity,
                VendorId = vendor.Id,
                CreatedAt = DateTime.UtcNow //Coordinated Universal Time
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Item added successfully!";
            return RedirectToAction("Index", "Vendor");
        }


    }
}