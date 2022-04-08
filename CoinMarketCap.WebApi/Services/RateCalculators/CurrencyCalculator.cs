using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoinMarketCap.WebApi.Services.RateCalculators
{
    public class CurrencyCalculator : IRateCalculator
    {
        public static List<string> GetSupportedPairs(IEnumerable<string> currency)
        {
            var list = new List<string>();
            list.AddRange(currency);
            list.Remove("BTC");
            return CoinMarketCapManager.GetCombinations(list);
        }

        public async Task<QuoteOutput> Calc(RateCollection rates, string pair)
        {
            var separatePair = pair.SeparatePair();
            var symbolVsBtc = rates.Rates.FirstOrDefault(x => x.Pair == $"BTC/{separatePair.symbol}");
            var currencyVsBtc = rates.Rates.FirstOrDefault(x => x.Pair == $"BTC/{separatePair.currency}");

            if (symbolVsBtc == null || currencyVsBtc == null)
                return null;

            return new QuoteOutput()
            {
                Symbol = separatePair.symbol,
                Currency = separatePair.currency,
                Price = currencyVsBtc.Price / symbolVsBtc.Price,
                Timestamp = rates.Timestamp
            };
        }
    }
}