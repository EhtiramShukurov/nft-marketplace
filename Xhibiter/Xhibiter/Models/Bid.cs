using Microsoft.EntityFrameworkCore;
using Xhibiter.Models.Base;

namespace Xhibiter.Models
{
    public class Bid:BaseEntity
    {
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public int AuctionId { get; set; }
        public Auction? Auction { get; set; }
        public int BidderId { get; set; }
        public UserProfile? Bidder { get; set; }
    }
}
