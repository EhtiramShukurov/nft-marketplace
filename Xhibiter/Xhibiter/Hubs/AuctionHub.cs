using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Xhibiter.DAL;

namespace Xhibiter.Hubs
{
    public class AuctionHub:Hub
    {
        AppDbContext _context { get; }
        public AuctionHub(AppDbContext context)
        {
            _context = context;
        }
        public async Task UpdateBid(string auctionId)
        {
            var auction = _context.Auctions.Include(a=>a.Winner).ThenInclude(a=>a.NFTUser).FirstOrDefault(a => a.Id == Convert.ToInt32(auctionId));
            decimal bidAmount = auction.CurrentPrice;
            string bidderName = auction.Winner.NFTUser.UserName;
            await Clients.All.SendAsync("bidUpdated",auctionId, bidAmount, bidderName);
        }

    }
}
