using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoinMarketCap.WebApi.Services.RateCalculators
{
    public class BtcCalculator : IRateCalculator
    {
        public static List<string> GetSupportedPairs(IEnumerable<string> symbols)
        {
            var list = new List<string>();
            list.AddRange(symbols);
            list.Remove("BTC");
            return CoinMarketCapManager.GetCombinations(list);
        }

        public async Task<QuoteOutput> Calc(RateCollection rates, string pair)
        {
            var separatePair = pair.SeparatePair();
            var symbolVsBtc = rates.Rates.FirstOrDefault(x => x.Pair == $"{separatePair.symbol}/BTC");
            var currencyVsBtc = rates.Rates.FirstOrDefault(x => x.Pair == $"{separatePair.currency}/BTC");

            if (symbolVsBtc == null || currencyVsBtc == null)
                return null;

            return new QuoteOutput()
            {
                Symbol = separatePair.symbol,
                Currency = separatePair.currency,
                Price = symbolVsBtc.Price / currencyVsBtc.Price,
                Timestamp = rates.Timestamp
            };
        }
    }
}