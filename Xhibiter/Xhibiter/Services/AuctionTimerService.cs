namespace Xhibiter.Services
{
    public class AuctionTimerService:IDisposable
    {
            private readonly Timer _timer;
            private readonly AuctionService _auctionService;

            public AuctionTimerService(AuctionService auctionService)
            {
                _auctionService = auctionService;
                _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromMinutes(1));
            }

            private async void DoWork(object state)
            {
                await _auctionService.CompleteEndedAuctionsAsync();
            }

            public void Dispose()
            {
                _timer.Dispose();
            }

    }
}
