using System.ComponentModel.DataAnnotations;
using Xhibiter.Models.Base;

namespace Xhibiter.Models
{
    public class UserProfile:BaseEntity
    {
        [StringLength(150)]
        public string? Bio { get; set; }
        [StringLength(100)]

        public string? ProfileImageUrl { get; set; }
        [StringLength(100)]

        public string? CoverImageUrl { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string NFTUserId { get; set; }
        [StringLength(100)]

        public string? InstagramLink { get; set; }
        [StringLength(150)]

        public string? TwitterLink { get; set; }
        [StringLength(100)]

        public string? ExternalLink { get; set; }
        public NFTUser? NFTUser { get; set; }
        public ICollection<NFTCollection>? Collections { get; set; }
        public ICollection<Bid>? Bids { get; set; }
        public ICollection<Auction>? WonAuctions { get; set; }
    }
}
