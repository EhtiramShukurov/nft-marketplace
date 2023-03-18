using System.ComponentModel.DataAnnotations.Schema;
using Xhibiter.Models.Base;

namespace Xhibiter.Models
{
    public class Activity:BaseEntity
    {
        public int Id { get; set; }
        public int? BidId { get; set; }
        public int? SaleId { get; set; }
        public Bid? Bid { get; set; }
        public Sale? Sale { get; set; }
        public DateTime Date { get; set; }
    }
}
