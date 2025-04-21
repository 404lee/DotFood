using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using DotFood.ViewModel;
using DotFood.Entity;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DotFood.Data;



namespace DotFood.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<Users> _userManager;
        private readonly SignInManager<Users> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UsersContext _db; 

        public AccountController(
            UserManager<Users> userManager,
            SignInManager<Users> signInManager,
            RoleManager<IdentityRole> roleManager,
            UsersContext db) 
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _db = db; 
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

        //index
        //[Authorize]
        [HttpGet]
        public async Task<IActionResult> EditProfile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var model = new EditProfileViewModel
            {
                Name = user.FullName,
                Country = user.Country,
                City = user.City
            };

            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfile(EditProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("EditProfile", model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            user.UserName = model.Name;
            user.FullName = model.Name;
            user.Country = model.Country;
            user.City = model.City;

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View("EditProfile", model);
            }

            TempData["SuccessMessage"] = "Profile updated successfully!";
            return RedirectToAction("EditProfile");
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> ChangePassword()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }
            var model = new ChangePasswordViewModel();

            return View(model);
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> ChangePassword([Bind(Prefix = "PasswordModel")] ChangePasswordViewModel model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return View("Index", new EditProfileViewModel { PasswordModel = model });
        //    }

        //    var user = await _userManager.GetUserAsync(User);
        //    if (user == null)
        //    {
        //        await _signInManager.SignOutAsync();
        //        return RedirectToAction("Login", "Account");
        //    }
        //    if (string.IsNullOrEmpty(model.CurrentPassword) ||
        //        string.IsNullOrEmpty(model.NewPassword) ||
        //        string.IsNullOrEmpty(model.ConfirmPassword))
        //    {
        //        ModelState.AddModelError(string.Empty, "All password fields are required");
        //        return View("Index", new EditProfileViewModel { PasswordModel = model });
        //    }
        //    if (model.NewPassword != model.ConfirmPassword)
        //    {
        //        ModelState.AddModelError(string.Empty, "The new password and confirmation password do not match.");
        //        return View("Index", new EditProfileViewModel { PasswordModel = model });
        //    }
        //    var result = await _userManager.ChangePasswordAsync(
        //        user,
        //        model.CurrentPassword,
        //        model.NewPassword);

        //    if (result.Succeeded)
        //    {
        //        await _signInManager.RefreshSignInAsync(user);
        //        TempData["StatusMessage"] = "Password changed successfully!";
        //        return RedirectToAction("Index");
        //    }
        //    AddErrors(result);
        //    return View("Index", new EditProfileViewModel { PasswordModel = model });
        //}
        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
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
