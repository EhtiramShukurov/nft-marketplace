namespace Xhibiter.ViewModels
{
    public class CreateCollectionVM
    {
        public string Name { get; set; }
        public IFormFile CoverImg { get; set; }
        public IFormFile LogoImg { get; set; }
        public IFormFile MainImg { get; set; }
        public string? Description { get; set; }
        public int CategoryId { get; set; }
    }
}
