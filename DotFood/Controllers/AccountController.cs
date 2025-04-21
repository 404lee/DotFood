using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using DotFood.ViewModel;
using DotFood.Entity;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;


namespace DotFood.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<Users> _userManager;
        private readonly SignInManager<Users> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountController(UserManager<Users> userManager, SignInManager<Users> signInManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new Users
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FullName = model.FullName,
                    Country = model.Country,
                    City = model.City,
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    if (!await _roleManager.RoleExistsAsync(model.Role))
                    {
                        await _roleManager.CreateAsync(new IdentityRole(model.Role)); 
                    }

                    await _userManager.AddToRoleAsync(user, model.Role);

                    return RedirectToAction("Login", "Account");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(model);
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, false);

                    if (result.Succeeded)
                    {
                        var role = (await _userManager.GetRolesAsync(user)).FirstOrDefault(); 
                        if (role == "admin")
                        {
                            return RedirectToAction("Index", "Admin");
                        }
                        else if (role == "vendor")
                        {
                            return RedirectToAction("Index", "Vendor");
                        }
                        else if (role == "customer")
                        {
                            return RedirectToAction("Index", "Customer");
                        }
                    }
                    ModelState.AddModelError("", "Invalid login attempt.");
                }
                else
                {
                    ModelState.AddModelError("", "User does not exist.");
                }
            }
            return View(model);
        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> EditProfile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            var model = new EditProfileViewModel
            {
                Name = user.UserName,
                Country = user.Country,
                City = user.City,
                EmailModel = new ChangeEmailViewModel { NewEmail = user.Email }
            };

            return View(model);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> EditProfile(EditProfileViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();
            
            user.UserName = model.Name;
            user.Country = model.Country;
            user.City = model.City;

            await _userManager.UpdateAsync(user);

            TempData["SuccessMessage"] = "Profile updated successfully";
            return RedirectToAction("EditProfile");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
