using System.ComponentModel.DataAnnotations;

namespace Xhibiter.ViewModels
{
    public class UserLoginVM
    {
        [Required]
        public string UsernameOrEmail { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string WalletPassword { get; set; }
        public bool IsPersistance { get; set; }
    }
}
