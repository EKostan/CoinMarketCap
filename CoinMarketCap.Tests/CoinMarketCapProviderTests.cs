using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace CoinMarketCap.Tests
{
    public class CoinMarketCapProviderTests
    {
        [Fact]
        public async Task GetQuotesTest()
        {
            var quoteOutputBtcString = File.ReadAllText("QuoteOutputBtc.json");
            var settings = new CoinMarketCapOption();
            var mockLogger = new Mock<ILogger<CoinMarketCapProvider>>();
            var coinMarketCapProvider = new CoinMarketCapProvider(mockLogger.Object, settings);

            var z = await coinMarketCapProvider.GetQuotes(
                string.Join(",", settings.Value.Symbols),
                settings.Value.Currency[0]);
        }
    }
}