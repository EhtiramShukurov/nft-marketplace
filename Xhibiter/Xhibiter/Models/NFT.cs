using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Xhibiter.Models.Base;

namespace Xhibiter.Models
{
    public class NFT:BaseEntity
    {
        [StringLength(30), Required]
        public string Name { get; set; }
        [StringLength(200), Required]
        public string Description { get; set; }
        public decimal Price { get; set; }
        public bool IsSale { get; set; }
        public bool IsAuctioned { get; set; }
        public string OwnerId { get; set; }
        public NFTUser? Owner { get; set; }
        public int MetadataId { get; set; }
        public NFTMetadata? Metadata { get; set; }
        public int CollectionId { get; set; }
        public NFTCollection? Collection { get; set; }
        public Auction? Auction { get; set; }
        public ICollection<NFTCategory>? NFTCategories { get; set; }
        public ICollection<Sale>? Sales { get; set; }
    }
}
