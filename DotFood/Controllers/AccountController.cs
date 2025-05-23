﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using DotFood.ViewModel;
using DotFood.Entity;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DotFood.Data;
using Microsoft.AspNetCore.Authentication;
using DotFood.Helpers;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace DotFood.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<Users> _userManager;
        private readonly SignInManager<Users> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UsersContext _db;
        private readonly JwtHelper _jwtHelper;


        public AccountController(
            UserManager<Users> userManager,
            SignInManager<Users> signInManager,
            RoleManager<IdentityRole> roleManager,  
            UsersContext db,
            JwtHelper jwtHelper)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _db = db;
            _jwtHelper = jwtHelper;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
    //    [ValidateAntiForgeryToken]
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
                    City = model.City
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                  
                    if (!await _roleManager.RoleExistsAsync(model.Role))
                    {
                        await _roleManager.CreateAsync(new IdentityRole(model.Role));
                    }

      
                    await _userManager.AddToRoleAsync(user, model.Role);


                    return RedirectToAction("Index", "Home");
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


        //[HttpGet]
        //public async Task<IActionResult> FixAdminPassword()
        //{
        //    var user = await _userManager.FindByEmailAsync("admin@example.com");

        //    if (user == null)
        //        return Content("Admin user not found.");

        //    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        //    var result = await _userManager.ResetPasswordAsync(user, token, "Admin@1234");

        //    if (result.Succeeded)
        //        return Content("✅ Admin password reset successfully.");

        //    return Content("❌ Password reset failed: " + string.Join(", ", result.Errors.Select(e => e.Description)));
        //}


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    var user = await _userManager.FindByEmailAsync(model.Email);
                    var roles = await _userManager.GetRolesAsync(user);
                    var role = roles.FirstOrDefault() ?? "Customer";

                    var token = _jwtHelper.GenerateToken(user.Id, user.Email, role);

                    Response.Cookies.Append("jwt", token, new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true,
                        SameSite = SameSiteMode.Strict,
                        Expires = DateTime.UtcNow.AddMinutes(30)
                    });

                    var handler = new JwtSecurityTokenHandler();
                    var jwtToken = handler.ReadJwtToken(token);
                    var identity = new ClaimsIdentity(jwtToken.Claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var principal = new ClaimsPrincipal(identity);

                    return RedirectToDashboard(role);
                }
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            }
            //return RedirectToAction("FixAdminPassword");
            return View(model);
        }


        private IActionResult RedirectToDashboard(string role)
        {
            return role switch
            {
                "Customer" => RedirectToAction("Index", "Customer"),
                "Vendor" => RedirectToAction("Index", "Vendor"),
                "Admin" => RedirectToAction("Index", "Admin"),
                _ => RedirectToAction("Index", "Home")
            };
        }

        [HttpGet]
        [Authorize]
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
        [Authorize]
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            Response.Cookies.Delete("Jwt");
            Response.Cookies.Delete("JwtToken");

            return RedirectToAction("Index", "Home");
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
   
                      ModelState.AddModelError(string.Empty, error.Description);   
            }
        }


    }
}