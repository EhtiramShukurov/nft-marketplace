using Microsoft.AspNetCore.Authorization;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Xhibiter.DAL;
using Xhibiter.Models;
using Xhibiter.ViewModels;

namespace Xhibiter.Areas.Manage.Controllers
{
    [Area("Manage")]
    [Authorize(Roles = "Admin")]

    public class AdminController : Controller
    {
        AppDbContext _context { get; }
        UserManager<NFTUser> _userManager { get; }
        SignInManager<NFTUser> _signInManager { get; }
        RoleManager<IdentityRole> _roleManager { get; }

        public AdminController(UserManager<NFTUser> userManager, SignInManager<NFTUser> signInManager, AppDbContext context, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _roleManager = roleManager;
        }
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(AdminLoginVM adminLogin,string? returnUrl)
        {
            if (!ModelState.IsValid) return View();
            NFTUser admin = await _userManager.FindByNameAsync(adminLogin.UsernameOrEmail);
            if (admin is null)
            {
                admin = await _userManager.FindByEmailAsync(adminLogin.UsernameOrEmail);
                if (admin is null)
                {
                    ModelState.AddModelError("", "Login or password is incorrect!");
                    return View();
                }
            }
            bool isAdmin = await _userManager.IsInRoleAsync(admin, "admin");
            if (!isAdmin)
            {
                ModelState.AddModelError("", "Access Denied!");
                return View();
            }
            var result = await _signInManager.PasswordSignInAsync(admin, adminLogin.Password, adminLogin.IsPersistance, true);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Login or password is incorrect!");
                return View();
            }
            if (returnUrl is null)
            {
                return RedirectToAction("Index", "Dashboard");
            }
            else
            {
                return Redirect(returnUrl);
            }
        }
        public async Task<IActionResult> CreateAdmin()
        {
            NFTUser admin = new NFTUser()
            {
                Email = "admin@gmail.com",
                UserName = "admin",
                WalletAddress = "admin",
                WalletPassword = "admin",
                EmailConfirmed = true,
            };
            var result = await _userManager.CreateAsync(admin, "Admin123!");
            if (!result.Succeeded)
            {
                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError("", item.Description);
                }
                return View();
            }
                return RedirectToAction(nameof(Login));
        }
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(Login));
        }
    }
}
