using System.ComponentModel.DataAnnotations;
using Xhibiter.Models.Base;

namespace Xhibiter.Models
{
    public class NFTCollection:BaseEntity
    {
        [StringLength(30), Required]
        public string Name { get; set; }
        [StringLength(100), Required]
        public string CoverImgUrl { get; set; }
        [StringLength(100), Required]

        public string LogoImgUrl { get; set; }
        [StringLength(100), Required]

        public string MainImgUrl { get; set; }
        [StringLength(150)]

        public string? Description { get; set; }
        public int CategoryId { get; set; }
        public DateTime CreationDate { get; set; }
        public Category? Category { get; set; }
        public int CreatorId { get; set; }
        public UserProfile? Creator { get; set; }
        public ICollection<NFT>? NFTs { get; set; }
        public ICollection<Sale>? Sales { get; set; }

    }
}
