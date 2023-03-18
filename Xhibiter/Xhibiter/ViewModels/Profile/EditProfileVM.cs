using Xhibiter.Models;

namespace Xhibiter.ViewModels
{
    public class EditProfileVM
    {
        public string? Bio { get; set; }
        public IFormFile? CoverImg { get; set; }
        public IFormFile? ProfileImg { get; set; }
        public string? CoverImgUrl { get; set; }
        public string? ProfileImgUrl { get; set; }
        public string? InstagramLink { get; set; }
        public string? TwitterLink { get; set; }
        public string? ExternalLink { get; set; }

        public string Username { get; set; }

    }
}
