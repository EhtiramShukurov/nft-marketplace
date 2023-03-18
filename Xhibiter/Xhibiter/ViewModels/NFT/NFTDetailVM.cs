using Xhibiter.Models;

namespace Xhibiter.ViewModels
{
    public class NFTDetailVM
    {
        public string CollectionName { get; set; }
        public NFT NFT { get; set; }
        public Auction Auction { get; set; }
        public IEnumerable<Sale> Sales { get; set; }
        public IEnumerable<Bid> Bids { get; set; }
        public IEnumerable<NFT> NFTs { get; set; }
    }
}
