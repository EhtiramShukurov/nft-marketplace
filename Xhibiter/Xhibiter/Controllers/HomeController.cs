using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xhibiter.DAL;
using Xhibiter.Models;
using Xhibiter.Utilities;
using Xhibiter.ViewModels;

namespace Xhibiter.Controllers
{
    public class HomeController : Controller
    {
        AppDbContext _context { get; }

        public HomeController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var collections = _context.NFTCollections.Include(n => n.Creator).ThenInclude(n => n.NFTUser).Include(n => n.NFTs).Include(n => n.Sales);
            decimal total = 0;
            var salesByCollection = new Dictionary<NFTCollection, decimal>();
            foreach (var item in collections)
            {
                total = item.Sales.Sum(s => s.SalePrice);
                if (total != 0)
                {
                salesByCollection.Add(item, total);
                }
            }
            var sortedSalesByCollection = salesByCollection.OrderByDescending(x => x.Value).Take(15).ToList();
            var auctions = _context.Auctions.Include(a => a.Winner).ThenInclude(a => a.NFTUser).Include(a => a.NFT).ThenInclude(a => a.Owner).ThenInclude(a => a.Profile).Where(a => !a.IsCompleted && a.EndTime < DateTime.Now);
            if (auctions.Count() > 0)
            {
                foreach (var item in auctions)
                {
                    if (item.EndTime < DateTime.Now && item.IsCompleted == false)
                    {
                        item.IsCompleted = true;
                        item.NFT.IsAuctioned = false;
                        if (item.WinnerId != item.NFT.Owner.Profile.Id)
                        {
                            await item.Winner.NFTUser.WalletAddress.TransferAsync(item.WinnerWallet, item.CurrentPrice, item.NFT.Owner.WalletAddress);
                            var sale = new Sale()
                            {
                                Date = DateTime.Now,
                                SalePrice = item.CurrentPrice,
                                CollectionId = item.NFT.CollectionId,
                                NFTId = item.NFT.Id,
                                From = item.Winner.NFTUser,
                                To = item.NFT.Owner,

                            };
                            item.NFT.Owner = item.Winner.NFTUser;
                             await _context.Sales.AddAsync(sale);
                        }
                    }
                }
                await _context.SaveChangesAsync();
            }
            HomeVM home = new()
            {
                Collections = _context.NFTCollections.Include(n => n.Creator).ThenInclude(n => n.NFTUser).Include(n => n.NFTs).OrderByDescending(n => n.Id).Take(10).ToList(),
                NFTs = _context.NFTs.Include(n => n.Metadata).ThenInclude(m=>m.Creator).ThenInclude(m=>m.Profile).Include(n => n.Auction).Take(20),
                Auctions = _context.NFTs.Include(n => n.Metadata).Include(n => n.Auction).Where(n=>n.Auction != null && n.IsAuctioned && !n.Auction.IsCompleted && !n.IsSale),
                Categories = _context.Categories,
                TopCollections = sortedSalesByCollection
            };
            return View(home);
        }
    }
}
