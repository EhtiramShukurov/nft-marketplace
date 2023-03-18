using Xhibiter.DAL;
using Xhibiter.Models;

namespace Xhibiter.Utilities
{
    public static class AuctionManagement
    {
        public static async Task CompleteAuction(this NFT nft,Auction auction,AppDbContext context)
        {
            if (auction.EndTime < DateTime.Now && auction.IsCompleted == false)
            {
                auction.IsCompleted = true;
                nft.IsAuctioned = false;
                if (auction.WinnerId != nft.Owner.Profile.Id)
                {
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
                    await context.Sales.AddAsync(sale);
                    await context.SaveChangesAsync();
                    await auction.Winner.NFTUser.WalletAddress.TransferAsync(auction.WinnerWallet, auction.CurrentPrice, nft.Owner.WalletAddress);
                }
            }
        }
    }
}
