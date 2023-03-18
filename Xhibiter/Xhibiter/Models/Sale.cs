using Xhibiter.Models.Base;

namespace Xhibiter.Models
{
    public class Sale:BaseEntity
    {
        public int NFTId { get; set; }
        public NFT? NFT { get; set; }
        public int CollectionId { get; set; }
        public NFTCollection? Collection { get; set; }
        public NFTUser? From { get; set; }
        public NFTUser? To { get; set; }
        public decimal SalePrice { get; set; }
        public DateTime Date { get; set; }
    }
}
