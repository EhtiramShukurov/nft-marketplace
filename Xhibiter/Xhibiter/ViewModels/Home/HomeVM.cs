using Xhibiter.Models;

namespace Xhibiter.ViewModels
{
    public class HomeVM
    {
        public IEnumerable<NFT>? NFTs{ get; set; }
        public IEnumerable<NFT>? Auctions{ get; set; }
        public List<NFTCollection>? Collections { get; set; }
        public IEnumerable<Category>? Categories { get; set; }
        public List<KeyValuePair<NFTCollection, decimal>> TopCollections { get; set; }

    }
}
