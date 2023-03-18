using Microsoft.EntityFrameworkCore;
using Xhibiter.DAL;
using Xhibiter.Models;
using Xhibiter.Utilities;

namespace Xhibiter.Services
{
    public class AuctionService
    {
            private readonly AppDbContext _context;

            public AuctionService(AppDbContext dbContext)
            {
                _context = dbContext;
            }

            public async Task CompleteEndedAuctionsAsync()
            {
            var auctions = _context.Auctions.Include(a => a.Winner).ThenInclude(a => a.NFTUser).Include(a => a.NFT).ThenInclude(a => a.Owner).ThenInclude(a => a.Profile).Where(a => !a.IsCompleted && a.EndTime < DateTime.Now);
            if (auctions.Count() > 0)
            {
                foreach (var item in auctions)
                {
                    Complete(item.NFT,item);
                }
            }
        }
            public async Task Complete( NFT nft, Auction auction)
            {
                if (auction.EndTime < DateTime.Now && auction.IsCompleted == false)
                {
                    auction.IsCompleted = true;
                    if (auction.WinnerId != nft.Owner.Profile.Id)
                    {
                        await auction.Winner.NFTUser.WalletAddress.TransferAsync(auction.WinnerWallet, auction.CurrentPrice, nft.Owner.WalletAddress);
                        var sale = new Sale()
                        {
                            Date = DateTime.Now,
                            SalePrice = auction.CurrentPrice,
                            CollectionId = nft.CollectionId,
                            NFTId = nft.Id,
                            From = auction.Winner.NFTUser,
                            To = nft.Owner,

                        };
                        nft.Owner = auction.Winner.NFTUser;
                        await _context.Sales.AddAsync(sale);
                        await _context.SaveChangesAsync();
                    }
                }
            }
    }
}
