using Xhibiter.Models.Base;

namespace Xhibiter.Models
{
    public class NFTProperty:BaseEntity
    {
        public int PropertyId { get; set; }
        public int NFTId { get; set; }
        public NFT? NFT { get; set; }
        public Property? Property { get; set; }
    }
}
