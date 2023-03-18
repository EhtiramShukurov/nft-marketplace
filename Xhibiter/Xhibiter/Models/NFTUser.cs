using Microsoft.AspNetCore.Identity;

namespace Xhibiter.Models
{
    public class NFTUser:IdentityUser
    {
        public int Id { get; set; }
        public string WalletAddress { get; set; }
        public string WalletPassword { get; set; }
        public UserProfile? Profile { get; set; }
        public ICollection<NFT>? NFTs { get; set; }
        public void SetWalletPassword(string password)
        {
            var passwordHasher = new PasswordHasher<NFTUser>();
            WalletPassword = passwordHasher.HashPassword(this, password);
        }
        
        public bool VerifyWalletPassword(string password)
        {
            var passwordHasher = new PasswordHasher<NFTUser>();
            return passwordHasher.VerifyHashedPassword(this, WalletPassword, password) != PasswordVerificationResult.Failed;
        }
    }
}
