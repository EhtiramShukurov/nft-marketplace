using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xhibiter.DAL;
using Xhibiter.Models;
using Xhibiter.ViewModels;

namespace Xhibiter.ViewComponents
{
    public class BidViewComponent:ViewComponent
    {
        AppDbContext _context { get; }
        UserManager<NFTUser> _userManager { get; }

        public BidViewComponent(AppDbContext context, UserManager<NFTUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public async Task<IViewComponentResult> InvokeAsync(int auctionId,string username)
        {
            UserProfile user = await _context.UserProfiles.FirstOrDefaultAsync(u => u.NFTUser.UserName == username);
            BidVM bid = new BidVM()
            {
                AuctionId = auctionId,
                BidderId = user.Id
            };
            return View(bid);
        }
    }
}
