using Xhibiter.Models;

namespace Xhibiter.ViewModels
{
    public class SearchVM
    {
        public IEnumerable<NFT> NFTs { get; set; }
        public IEnumerable<UserProfile> UserProfiles { get; set; }
        public IEnumerable<NFTCollection> Collections { get; set; }
        public string? Query { get; set; }
    }
}
