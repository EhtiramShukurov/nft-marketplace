namespace Xhibiter.ViewModels
{
    public class EditNFTVM
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string ExternalLink { get; set; }
        public bool IsSale { get; set; }
        public IFormFile? Media { get; set; }
        public List<int>? PropertyIds { get; set; }
        public int CollectionId { get; set; }
        public List<int>? CategoryIds { get; set; }
    }
}
