using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Nethereum.Web3;
using Xhibiter.DAL;
using Xhibiter.Models;
using Xhibiter.Utilities;
using Xhibiter.ViewModels;

namespace Xhibiter.ViewComponents
{
    public class ProfileViewComponent:ViewComponent
    {
        AppDbContext _context { get; }
        UserManager<NFTUser> _userManager { get; }

        public ProfileViewComponent(AppDbContext context, UserManager<NFTUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public async Task<IViewComponentResult> InvokeAsync(string username)
        {
            NFTUser user = await _userManager.FindByNameAsync(username);
            Web3 web3 = new Web3();
            UserInfoVM userinfo = new UserInfoVM()
            {
                Username = user.UserName,
                WalletAddress = user.WalletAddress,
                Balance = await user.WalletAddress.AccountBalanceAsync(web3)
            };
            return View(userinfo);
        }
    }
}
