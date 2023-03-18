using Xhibiter.Models;

namespace Xhibiter.ViewModels
{
    public class BidVM
    {
        public decimal Amount { get; set; }
        public int AuctionId { get; set; }
        public int BidderId { get; set; }
        public string BidderWalletinfo { get; set; }
    }
}
