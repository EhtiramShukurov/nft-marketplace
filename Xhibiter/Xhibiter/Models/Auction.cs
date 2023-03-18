using System.ComponentModel.DataAnnotations;
using Xhibiter.Models.Base;

namespace Xhibiter.Models
{
    public class Auction:BaseEntity
    {
        [Required]

        public decimal StartingPrice { get; set; }
        public decimal CurrentPrice { get; set; }
        public decimal MinimumIncrement { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public bool IsCompleted { get; set; }
        public int NFTId { get; set; }
        public NFT? NFT { get; set; }
        public int WinnerId { get; set; }
        public UserProfile? Winner { get; set; }
        public ICollection<Bid>? Bids { get; set; }
        public string? WinnerWallet { get; set; }
    }
}
