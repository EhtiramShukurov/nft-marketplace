using Microsoft.AspNetCore.Mvc;
using Xhibiter.Models;
using Xhibiter.ViewModels;
using Xhibiter.Utilities;
using Nethereum.Web3;
using Microsoft.AspNetCore.Identity;
using Xhibiter.DAL;
using Xhibiter.Utilities.Enums;
using Xhibiter.Abstractions.Services;
using System.IO;

namespace Xhibiter.Controllers
{
    public class AccountController : Controller
    {
        AppDbContext _context { get; }
        UserManager<NFTUser> _userManager { get; }
        SignInManager<NFTUser> _signInManager { get; }
        RoleManager<IdentityRole> _roleManager { get; }
        IEmailService _emailService { get; }
        IWebHostEnvironment _env { get; }


        public AccountController(UserManager<NFTUser> userManager, SignInManager<NFTUser> signInManager, AppDbContext context, RoleManager<IdentityRole> roleManager, IEmailService emailService, IWebHostEnvironment env)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _roleManager = roleManager;
            _emailService = emailService;
            _env = env;
        }
        public  IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(UserRegisterVM registerVM)
        {
            if (!ModelState.IsValid) return View();
            NFTUser user = new NFTUser()
            {
                Email = registerVM.Email,
                UserName = registerVM.Username,
            };
            user.SetWalletPassword(registerVM.WalletPassword);
            Web3 web3 = new Web3();
            string WalletAddress = await AccountManagement.AccountCreationAsync(registerVM.WalletPassword, web3);
            user.WalletAddress = WalletAddress;
            var result = await _userManager.CreateAsync(user, registerVM.Password);
            if (!result.Succeeded)
            {
                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError("", item.Description);
                }
                return View();
            }
            await _userManager.AddToRoleAsync(user, Roles.Member.ToString());
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var confirmationLink = Url.Action("ConfirmEmail", "Account", new { token, Email = user.Email }, Request.Scheme);
            _emailService.Send(user.Email, "Confirm your Account", confirmationLink);
            return RedirectToAction(nameof(SuccessfullyRegistered));
        }
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(UserLoginVM loginVM, string? ReturnUrl)
        {
            if (!ModelState.IsValid) return View();
            NFTUser user = await _userManager.FindByNameAsync(loginVM.UsernameOrEmail);
            if (user is null)
            {
                user = await _userManager.FindByEmailAsync(loginVM.UsernameOrEmail);
                if (user is null)
                {
                    ModelState.AddModelError("", "Login or password is incorrect!");
                    return View();
                }
            }
            if (!user.VerifyWalletPassword(loginVM.WalletPassword))
            {
                ModelState.AddModelError("", "Login or password is incorrect!");
                return View();
            }
            var result = await _signInManager.PasswordSignInAsync(user, loginVM.Password, loginVM.IsPersistance, true);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Login or password is incorrect!");
                return View();
            }
            if (ReturnUrl is null)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return Redirect(ReturnUrl);
            }
        }
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
        public async Task<IActionResult> AddRoles()
        {
            foreach (var item in Enum.GetValues(typeof(Roles)))
            {
                if (!await _roleManager.RoleExistsAsync(item.ToString()))
                {
                    await _roleManager.CreateAsync(new IdentityRole { Name = item.ToString() });
                }
            }
            return RedirectToAction("Index", "Home");
        }
        public async Task<IActionResult> ConfirmEmail(string token, string email)
        {
            NFTUser user = await _userManager.FindByEmailAsync(email);
            if (user == null) return NotFound();
            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (!result.Succeeded)
            {
                return NotFound();
            }
            CopyFile("default-cover.jpg", Path.Combine(_env.WebRootPath, "assets", "images", "users", user.UserName, "cover"));
            CopyFile("default-profile.jpg", Path.Combine(_env.WebRootPath, "assets", "images", "users", user.UserName, "profile"));
            var UserProfile = new UserProfile()
            {
                CreatedDate = DateTime.Now,
                CoverImageUrl = "default-cover.jpg",
                ProfileImageUrl = "default-profile.jpg"
            };
            UserProfile.NFTUser = user;
            user.Profile = UserProfile;
            await _context.UserProfiles.AddAsync(UserProfile);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Login));
        }
        public IActionResult SuccessfullyRegistered()
        {
            return View();
        }
        public void CopyFile(string name,string destination)
        {
            string sourcePath = Path.Combine(_env.WebRootPath,"assets","images","default",name);
            string filePath = Path.Combine(destination, Path.GetFileName(sourcePath));
            string destinationPath = Path.Combine(destination, name);
            if (!Directory.Exists(destination))
            {
                Directory.CreateDirectory(destination);
            }
            if (!System.IO.File.Exists(destinationPath))
            {
                using (System.IO.File.Create(destinationPath)) { }
            }

            System.IO.File.Copy(sourcePath, destinationPath, true);
        }
    }
}
