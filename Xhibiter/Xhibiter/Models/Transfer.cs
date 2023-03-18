using Xhibiter.Models.Base;

namespace Xhibiter.Models
{
    public class Transfer : BaseEntity
    {
        public NFTUser? From { get; set; }
        public NFTUser? To { get; set; }
        public decimal TransferAmount { get; set; }
        public DateTime Date { get; set; }
    }
}
