

namespace FlipalooWeb.Background
{
    public class BackgroundWork : BackgroundService
    {
        readonly ILogger<BackgroundWork> _logger;

        public BackgroundWork(ILogger<BackgroundWork> logger)
        {
            _logger = logger;
        }

        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                OddsFinder oddsFinder = new OddsFinder();
                //oddsFinder.FindOdds();
                await Task.Delay(TimeSpan.FromMinutes(1));
            }
        }
    }
}
