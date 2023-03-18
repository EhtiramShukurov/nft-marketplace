using System.ComponentModel.DataAnnotations;

namespace Xhibiter.ViewModels
{
    public class AdminLoginVM
    {
        [Required]
        public string UsernameOrEmail { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public bool IsPersistance { get; set; }
    }
}
