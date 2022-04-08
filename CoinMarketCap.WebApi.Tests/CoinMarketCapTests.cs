using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CoinMarketCap.Dal;
using CoinMarketCap.WebApi.Services;
using CoinMarketCap.WebApi.Services.RateCalculators;
using CoinMarketCap.WebApi.Services.RateCalculators.Burse;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using Xunit;

namespace CoinMarketCap.WebApi.Tests
{
    public class CoinMarketCapTests
    {
        [Fact]
        public async Task CalculateTest()
        {
            var rateCollectionString = File.ReadAllText("RateCollection.json");
            var rateCollection = JsonConvert.DeserializeObject<RateCollection>(rateCollectionString);
            var options = new CoinMarketCapOption();
            var settings = options.Value;

            var coinMarketCapProvider = new CoinMarketCapProvider(new Mock<ILogger<CoinMarketCapProvider>>().Object, options);
            var mockCoinMarketCapContext = new Mock<ICoinMarketCapContext>();
            var mockLogger = new Mock<ILogger<CoinMarketCapManager>>();
            var coinMarketCapManager = new CoinMarketCapManager(
                coinMarketCapProvider,
                mockCoinMarketCapContext.Object,
                options,
                mockLogger.Object);

            var symbols = new List<string>();
            symbols.AddRange(settings.Symbols);
            symbols.AddRange(settings.Currency);
            symbols.AddRange(settings.CustomSymbols);
            var combinations = CoinMarketCapManager.GetCombinations(symbols);

            RateCalculatorRegistry.AddCalculators(BtcCalculator.GetSupportedPairs(settings.Symbols), new BtcCalculator());
            RateCalculatorRegistry.AddCalculators(RevertCurrencyCalculator.GetSupportedPairs(settings.Symbols, settings.Currency), new RevertCurrencyCalculator());
            RateCalculatorRegistry.AddCalculators(CurrencyCalculator.GetSupportedPairs(settings.Currency), new CurrencyCalculator());
            //RateCalculatorRegistry.AddCalculators(ProofCalculator.GetSupportedPairs(settings.Symbols, settings.Currency), new ProofCalculator());

            var calculateRates = await coinMarketCapManager.Calculate(
                rateCollection, 
                new List<Quote>(),
                new HashSet<string>(combinations));

            var etalonPrices = ReadEtalon();

            foreach (var quote in calculateRates.Quotes)
            {
                if (quote.Pair == "XEM/PROOF")
                {

                }

                var price = etalonPrices[quote.Pair];
                var quotePrice = Math.Round(quote.Price, 8);
                Assert.Equal(price, quotePrice);
            }

            foreach (var etalonPrice in etalonPrices)
            {
                if (etalonPrice.Key == "BTC/BTC")
                {
                    continue;
                }

                var rate = calculateRates.Quotes.FirstOrDefault(x => x.Pair == etalonPrice.Key);
                Assert.NotNull(rate);
            }
        }

        private Dictionary<string, decimal> ReadEtalon()
        {
            var etalonPairs = new Dictionary<string, decimal>();
            foreach (var line in File.ReadAllText("Etalon.txt").Split(Environment.NewLine))
            {
                if(string.IsNullOrEmpty(line))
                    continue;

                var columns = line.Split("\t", StringSplitOptions.RemoveEmptyEntries);
                etalonPairs[columns[0]] = Convert.ToDecimal(columns[1]);
            }

            return etalonPairs;
        }
    }
}