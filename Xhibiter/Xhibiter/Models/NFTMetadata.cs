using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using Xhibiter.Models.Base;

namespace Xhibiter.Models
{
    public class NFTMetadata : BaseEntity
    {
        private string _tokenId;

        public string TokenId
        {
            get { return _tokenId; }
            set { _tokenId = HashTokenId(); }
        }
        [StringLength(100), Required]

        public string MediaUrl { get; set; }
        public string MediaType { get; set; }
        public string? ExternalLink { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatorId { get; set; }
        public int NFTId { get; set; }
        public NFTUser? Creator { get; set; }
        public NFT? NFT { get; set; }

        private static string HashTokenId()
        {
            Guid tokenIdGuid = Guid.NewGuid();
            byte[] tokenIdBytes = tokenIdGuid.ToByteArray();
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(tokenIdBytes);
                string hashString = BitConverter.ToString(hashBytes).Replace("-", "");
                return hashString;
            }
        }
    }
}
