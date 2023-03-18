using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xhibiter.DAL;
using Xhibiter.Models;

namespace Xhibiter.Controllers
{
    public class RankingController : Controller
    {
        AppDbContext _context { get; }

        public RankingController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var collections = _context.NFTCollections.Include(n => n.NFTs).Include(n => n.Creator).ThenInclude(n => n.NFTUser).Include(n => n.NFTs).Include(n=>n.Sales);
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
            var sortedSalesByCollection = salesByCollection.OrderByDescending(x => x.Value).ToList();
            return View(sortedSalesByCollection);
        }
        public async Task RecordSale(int nftId, decimal salePrice)
        {
            var nft = await _context.NFTs.FindAsync(nftId);
            var sale = new Sale
            {
                NFTId = nftId,
                CollectionId = nft.CollectionId,
                SalePrice = salePrice,
                Date = DateTime.Now
            };

            _context.Sales.Add(sale);
            await _context.SaveChangesAsync();
        }
        public async Task<decimal> GetTotalSalesForCollection(int collectionId)
        {
            return await _context.Sales
                .Where(s => s.CollectionId == collectionId)
                .SumAsync(s => s.SalePrice);
        }

    }
}
