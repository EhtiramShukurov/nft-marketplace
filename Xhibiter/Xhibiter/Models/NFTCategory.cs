using Xhibiter.Models.Base;

namespace Xhibiter.Models
{
    public class NFTCategory:BaseEntity
    {
        public int NFTId { get; set; }
        public int CategoryId { get; set; }
        public NFT? NFT { get; set; }
        public Category? Category { get; set; }
    }
}
