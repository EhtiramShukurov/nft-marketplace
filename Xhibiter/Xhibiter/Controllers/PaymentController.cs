using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nethereum.Web3;
using Xhibiter.DAL;
using Xhibiter.Models;
using Xhibiter.Utilities;
using Xhibiter.ViewModels;

namespace Xhibiter.Controllers
{
    public class PaymentController : Controller
    {
        IConfiguration _config { get; }
        AppDbContext _context { get; }
        UserManager<NFTUser> _userManager { get; }
        public PaymentController(AppDbContext context, UserManager<NFTUser> userManager, IConfiguration config)
        {
            _context = context;
            _userManager = userManager;
            _config = config;
        }
        [HttpPost]
        public async Task<IActionResult> PlaceBid(string auctionId, string username)
        {
            UserProfile user = await _context.UserProfiles.FirstOrDefaultAsync(u => u.NFTUser.UserName == username);
            BidVM bid = new BidVM()
            {
                AuctionId = Convert.ToInt32(auctionId),
                BidderId = user.Id,
            };
            return PartialView("_BidPartial", bid);
        }
        [HttpPost]
        public async Task<IActionResult> BuyNFT(PurchaseVM purchaseVM)
        {
            var nft = await _context.NFTs.Include(n=>n.Owner).Include(n=>n.Sales).FirstOrDefaultAsync(n => n.Id == purchaseVM.NFTId);
            if (nft is null)
            {
                return Json(new { success = false, message = "NFT not found." });
            }
            var buyer = await _context.NFTUsers.Include(n => n.Profile).FirstOrDefaultAsync(n => n.UserName == User.Identity.Name);

            if (buyer is null||buyer.Profile is null)
            {
                return Json(new { success = false, message = "User not found." });
            }
            if (nft.Owner.UserName == User.Identity.Name)
            {
                return Json(new { success = false, message = "You are the owner of this nft." });
            }
            if (!buyer.VerifyWalletPassword(purchaseVM.WalletInfo))
            {
                return Json(new { success = false, message = "Wallet info not found." });
            }
            Web3 web3 = new Web3();
            var balance = await buyer.WalletAddress.AccountBalanceAsync(web3);
            if (purchaseVM.Price > balance)
            {
                return Json(new { success = false, message = "Insufficient Balance." });
            }
            var sale = new Sale()
            {
                CollectionId = nft.CollectionId,
                Date = DateTime.Now,
                SalePrice = purchaseVM.Price,
                NFTId = nft.Id,
                From = nft.Owner,
                To = buyer
            };
            var activity = new Activity()
            {
                Sale = sale,
                Date = sale.Date
            };
            await buyer.WalletAddress.TransferAsync(purchaseVM.WalletInfo, purchaseVM.Price, nft.Owner.WalletAddress);
            nft.IsSale = false;
            var a = await _userManager.GetUserIdAsync(buyer);
            nft.OwnerId = a;
            await _context.Sales.AddAsync(sale);
            await _context.Activities.AddAsync(activity);
            var result = await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Purchase was successful!" });
        }
        [HttpPost]
        public async Task<IActionResult> AddBalance(string recipient,decimal price,string username)
        {
            var address = _config.GetSection("TestAccount")["Address"];
            var password = _config.GetSection("TestAccount")["Password"];
            await address.TransferAsync(password, price, recipient);
            return RedirectToAction("Index","Profile",new {username = username});
        }
    }
}
