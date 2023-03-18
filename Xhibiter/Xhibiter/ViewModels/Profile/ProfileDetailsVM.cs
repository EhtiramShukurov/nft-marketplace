using Xhibiter.Models;

namespace Xhibiter.ViewModels
{
    public class ProfileDetailsVM
    {
        public IEnumerable<NFT> OnSaleNFTs { get; set; }
        public IEnumerable<NFT> CreatedNFTs { get; set; }
        public IEnumerable<NFT> AuctionedNFTs { get; set; }
        public IEnumerable<NFT> HiddenNFTs { get; set; }
        public IEnumerable<NFTCollection> Collections { get; set; }
        public UserProfile User { get; set; }
        public IEnumerable<Activity> Activities { get; set; }

    }
}
