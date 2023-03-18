namespace Xhibiter.ViewModels
{
    public class EditCollectionVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public IFormFile? CoverImg { get; set; }
        public IFormFile?LogoImg { get; set; }
        public IFormFile? MainImg { get; set; }
        public int CategoryId { get; set; }
    }
}
