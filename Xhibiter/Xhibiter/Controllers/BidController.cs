using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nethereum.Web3;
using Xhibiter.DAL;
using Xhibiter.Models;
using Xhibiter.Utilities;
using Xhibiter.ViewModels;

namespace Xhibiter.Controllers
{
    public class BidController : Controller
    {
        AppDbContext _context { get; }
        public BidController(AppDbContext context)
        {
            _context = context;
        }
        [HttpPost]
        public async Task<IActionResult> PlaceBid(BidVM bidVM)
        {
            var auction = await _context.Auctions.Include(a=>a.NFT).ThenInclude(n=>n.Owner)
                .FirstOrDefaultAsync(a=>a.Id == bidVM.AuctionId && !a.IsCompleted);
            if (auction is null || bidVM.Amount <= auction.CurrentPrice)
            {
                return Json(new { success = false, message = "Invalid bid." });
            }
            if (auction.NFT.Owner.UserName == User.Identity.Name)
            {
                return Json(new { success = false, message = "You are the owner of this nft." });
            }
            if (auction.EndTime <= DateTime.Now)
            {
                return Json(new {success = false,message = "Auction has already ended." });
            }
            var user = await _context.NFTUsers.Include(n=>n.Profile).FirstOrDefaultAsync(n => n.UserName == User.Identity.Name);
            if (user is null ||user.Profile is null)
            {
                return Json(new { success = false, message = "User not found." });
            }
            if (auction.WinnerId == user.Profile.Id )
            {
                return Json(new { success = false, message = "You are already the highest bidder." });
            }
            if (!user.VerifyWalletPassword(bidVM.BidderWalletinfo))
            {
                return Json(new { success = false, message = "Wallet info not found." });
            }
            Web3 web3 = new Web3();
            var balance = await user.WalletAddress.AccountBalanceAsync(web3);
            if (bidVM.Amount > balance)
            {
                return Json(new { success = false, message = "Insufficient Balance." });
            }
            var bid = new  Bid()
            {
                BidderId = user.Profile.Id,
                AuctionId = auction.Id,
                Amount = bidVM.Amount,
                Date = DateTime.Now,
            };
            auction.CurrentPrice = bidVM.Amount;
            auction.WinnerId = user.Profile.Id;
            if (auction.NFT.Owner != user)
            {
                auction.WinnerWallet = bidVM.BidderWalletinfo;
            };
            var activity = new Activity()
            {
                Bid = bid,
                Date = bid.Date
            };
            await _context.Bids.AddAsync(bid);
            await _context.Activities.AddAsync(activity);
            await _context.SaveChangesAsync();
            return Json(new { success = true ,message = "Bid placed succesfully!"});
        }

    }
}
