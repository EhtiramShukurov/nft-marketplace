using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xhibiter.DAL;
using Xhibiter.Models;
using Xhibiter.ViewModels;

namespace Xhibiter.Controllers
{
    
    public class AuctionController : Controller
    {
        AppDbContext _context { get; }

        public AuctionController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Create(int? id)
        {
            if(id is null|| id <= 0) return BadRequest();
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(int? id,CreateAuctionVM createAuction)
        {
            if (id is null || id <= 0) return BadRequest();
            if (!ModelState.IsValid)
            {
                return View();
            }
            if (createAuction.StartingPrice <=0)
            {
                ModelState.AddModelError("StartingPrice", "Start price must be higher than 0.");
                return View();

            }
            if (createAuction.MinimumIncrement <= 0)
            {
                ModelState.AddModelError("MinimumIncrement", "Increment for bids must be higher than 0.");
                return View();

            }
            if (createAuction.EndTime <= DateTime.Now)
            {
                ModelState.AddModelError("EndTime", "Auction should take at least one hour.");
                return View();

            }

            NFT nft = await _context.NFTs.Include(n=>n.Owner).ThenInclude(n=>n.Profile).FirstOrDefaultAsync(n => n.Id == id);
            if(nft is null) return NotFound();
            if (nft.IsAuctioned)
            {
                ModelState.AddModelError("", "This item is already on auction!");
                return View();
            }
            if (nft.IsSale)
            {
                ModelState.AddModelError("", "This item is on sale!");
                return View();
            }
            Auction auction = new Auction()
            {
                NFTId = nft.Id,
                StartTime = DateTime.Now,
                EndTime = createAuction.EndTime,
                StartingPrice = createAuction.StartingPrice,
                MinimumIncrement = createAuction.MinimumIncrement,
                CurrentPrice = createAuction.StartingPrice,
                WinnerId = nft.Owner.Profile.Id
            };
            nft.IsAuctioned = true;
            if (_context.Auctions.Any(a=>a.NFTId == nft.Id))
            {
                Auction exist = _context.Auctions.FirstOrDefault(a=>a.NFTId == nft.Id);
                exist.StartTime = DateTime.Now;
                exist.EndTime = createAuction.EndTime;
                exist.StartingPrice = createAuction.StartingPrice;
                exist.MinimumIncrement = createAuction.MinimumIncrement;
                exist.CurrentPrice = createAuction.StartingPrice;
                exist.WinnerId = nft.Owner.Profile.Id;
                exist.IsCompleted = false;
            }
            else
            {
            await _context.Auctions.AddAsync(auction);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Profile", new { username = User.Identity.Name });
        }
        public IActionResult Remove(int? id)
        {
            if (id is null || id <= 0) return BadRequest();
            NFT nft =  _context.NFTs.Include(n => n.Owner).ThenInclude(n => n.Profile).FirstOrDefault(n => n.Id == id);
            if (nft is null) return NotFound();

            Auction auction = _context.Auctions.Include(a => a.NFT).FirstOrDefault(a=>a.NFTId == id);
            if (auction is null) return NotFound();
            _context.Auctions.Remove(auction);
            nft.IsAuctioned = false;
            _context.SaveChanges();
            return RedirectToAction("Index", "Profile",new {username = User.Identity.Name});
        }
    }
}
