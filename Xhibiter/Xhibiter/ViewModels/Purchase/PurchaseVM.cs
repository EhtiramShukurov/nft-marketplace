namespace Xhibiter.ViewModels
{
    public class PurchaseVM
    {
        public string BuyerName { get; set; }
        public string OwnerId { get; set; }
        public decimal Price { get; set; }
        public int NFTId { get; set; }
        public string? NFTName { get; set; }
        public string? WalletInfo { get; set; }
    }
}
