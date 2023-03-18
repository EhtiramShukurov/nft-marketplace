using Xhibiter.Models;

namespace Xhibiter.Areas.Manage.ViewModels
{
    public class NFTDetailsVM
    {
        public IEnumerable<Auction> Auctions { get; set; }
        public IEnumerable<Sale> Sales { get; set; }
        public NFTMetadata Metadata { get; set; }
    }
}
