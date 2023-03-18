using Xhibiter.Models;

namespace Xhibiter.Areas.Manage.ViewModels
{
    public class UserDetailVM
    {
        public IEnumerable<NFT> NFTs { get; set; }
        public IEnumerable<NFTCollection> Collections { get; set; }
        public IEnumerable<Sale> Sales { get; set; }
        public IEnumerable<Sale> Purchases { get; set; }
        public UserProfile User { get; set; }
    }
}
