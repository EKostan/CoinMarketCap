using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinMarketCap.Dal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CoinMarketCap.WebApi.Services
{
    public class QuoteUpdaterService : PeriodicService
    {
        private readonly CoinMarketCapManager _coinMarketCapManager;
        private readonly IServiceScopeFactory _scopeFactory;

        public QuoteUpdaterService(
            CoinMarketCapManager coinMarketCapManager,
            IOptions<QuoteUpdaterSettings> settings,
            IServiceScopeFactory scopeFactory,
            ILogger<QuoteUpdaterService> logger)
            : base(settings.Value.SleepTime, logger)
        {
            _coinMarketCapManager = coinMarketCapManager;
            _scopeFactory = scopeFactory;
        }

        protected override async Task Do()
        {
            //using (var scope = _scopeFactory.CreateScope())
            //using (var context = scope.ServiceProvider.GetRequiredService<CoinMarketCapContext>())
            //{
            //    //var endedEventRequests = context.Quotes.ToList();

                await _coinMarketCapManager.UpdateQuotes();


                //await context.SaveChangesAsync();
            //}
        }
    }
}