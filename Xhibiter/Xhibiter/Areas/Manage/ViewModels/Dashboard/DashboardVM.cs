using Xhibiter.Models;

namespace Xhibiter.Areas.Manage.ViewModels
{
    public class DashboardVM
    {
        public IEnumerable<Sale> Sales { get; set; }
        public int UserNumber { get; set; }
        public int NFTNumber { get; set; }
        public int CollectionNumber { get; set; }
        public int SaleNumber { get; set; }

    }
}
