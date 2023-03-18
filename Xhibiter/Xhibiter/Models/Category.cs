using System.ComponentModel.DataAnnotations;
using Xhibiter.Models.Base;

namespace Xhibiter.Models
{
    public class Category:BaseEntity
    {
        [StringLength(20), Required]
        public string Name { get; set; }
        public ICollection<NFTCategory>? NFTCategories { get; set; }
        public ICollection<NFTCollection>? Collections { get; set; }
    }
}
