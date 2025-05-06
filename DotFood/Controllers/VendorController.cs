using DotFood.Data;
using DotFood.Entity;
using DotFood.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System;
using DotFood.ViewModel;

namespace DotFood.Controllers
{
    [Authorize( Roles = "Vendor")]

    public class VendorController : Controller
    {
        private readonly UsersContext _context;
        private readonly UserManager<Users> _userManager;
        private readonly IWebHostEnvironment _environment;
        public VendorController(UsersContext context, UserManager<Users> userManager
            ,IWebHostEnvironment environment)
        {
            _context = context;
           _userManager = userManager;
            _environment = environment;
        }

        public async Task<IActionResult> Index()
        {
            var vendor = await _userManager.GetUserAsync(User);
            var products = await _context.Products
                .Where(p => p.VendorId == vendor.Id)
                .ToListAsync();

            return View(products);
        }

        public async Task<IActionResult> ViewStore()
        {
            var vendor = await _userManager.GetUserAsync(User);

            var products = await _context.Products
                .Include(p => p.Category)
                .Where(p => p.VendorId == vendor.Id)
                .ToListAsync();

            var vendorStatus = await _context.VendorStatus
                .FirstOrDefaultAsync(v => v.UserId == vendor.Id);

            ViewBag.StoreStatus = vendorStatus?.Status ?? StoreStatus.Closed;

            return View(products);
        }


        [HttpGet]
        public async Task<IActionResult> AddItem()
        {
            var categories = await _context.Category.ToListAsync();

            var model = new ProductViewModel2
            {
                Categories = categories
            };
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> EditItem(int id)
        {
            var vendor = await _userManager.GetUserAsync(User);
            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.Id == id && p.VendorId == vendor.Id);

            if (product == null)
                return NotFound();

            var categories = await _context.Category.ToListAsync();

            var model = new ProductViewModel
            {
                Id = (int)product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Quantity = product.Quantity,
                CategoryId = product.CategoryId,
                Categories = categories
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditItem(ProductViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Categories = await _context.Category.ToListAsync();
                return View(model);
            }

            var vendor = await _userManager.GetUserAsync(User);
            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.Id == model.Id && p.VendorId == vendor.Id);

            if (product == null)
                return NotFound();

            product.Name = model.Name;
            product.Description = model.Description;
            product.Price = model.Price;
            product.Quantity = model.Quantity;
            product.CategoryId = model.CategoryId;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Product updated successfully!";
            return RedirectToAction("ViewStore");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteItem(int id)
        {
            var vendor = await _userManager.GetUserAsync(User);
            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.Id == id && p.VendorId == vendor.Id);

            if (product == null)
                return NotFound();

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Product deleted successfully!";
            return RedirectToAction("ViewStore");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddItem(ProductViewModel2 model)
        {
            
            var wwwroot = _environment.WebRootPath + "/Images/";

            if (model.ImageFile != null && model.ImageFile.Length > 0)
            {
                var extension = Path.GetExtension(model.ImageFile.FileName).ToLowerInvariant();


                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".jfif" };

                if (!allowedExtensions.Contains(extension))
                {
                    ModelState.AddModelError("ImageFile", "Only image files (.jpg, .jpeg, .png, .gif, .jfif) are allowed.");
                }

                Guid guid = Guid.NewGuid();

                string fullPath = System.IO.Path.Combine(wwwroot, guid + model.ImageFile.FileName);

                if (allowedExtensions.Contains(extension))
                {
                    using (var fileStream = new FileStream(fullPath, FileMode.Create))
                    {
                        model.ImageFile.CopyTo(fileStream);
                    }
                }

                model.imageName = guid + model.ImageFile.FileName;
            }
            else 
            {
                model.imageName = "";
            }
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
                CreatedAt = DateTime.UtcNow, //Coordinated Universal Time
                imageName = model.imageName
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Item added successfully!";
            return RedirectToAction("Index", "Vendor");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStoreStatus()
        {
            var vendor = await _userManager.GetUserAsync(User);

            var statusEntry = await _context.VendorStatus
                .FirstOrDefaultAsync(v => v.UserId == vendor.Id);

            if (statusEntry == null)
            {
                statusEntry = new VendorStatus
                {
                    UserId = vendor.Id,
                    Status = StoreStatus.Closed
                };
                _context.VendorStatus.Add(statusEntry);
            }

            statusEntry.Status = statusEntry.Status == StoreStatus.Open ? StoreStatus.Closed : StoreStatus.Open;
            await _context.SaveChangesAsync();

            return RedirectToAction("ViewStore");
        }


    }
}