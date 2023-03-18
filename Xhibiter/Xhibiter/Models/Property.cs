using Xhibiter.Models.Base;

namespace Xhibiter.Models
{
    public class Property:BaseEntity
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public ICollection<NFTProperty>? NFTProperties { get; set; }
    }
}
