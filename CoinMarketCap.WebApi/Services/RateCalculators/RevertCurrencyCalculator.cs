using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoinMarketCap.WebApi.Services.RateCalculators
{
    public class RevertCurrencyCalculator : IRateCalculator
    {
        public static List<string> GetSupportedPairs(IEnumerable<string> symbols, IEnumerable<string> currency)
        {
            var list = new List<string>();

            foreach (var itemCurrency in currency)
            {
                foreach (var itemSymbol in symbols)
                {
                    list.Add($"{itemCurrency}/{itemSymbol}");
                }
            }

            return list;
        }

        public async Task<QuoteOutput> Calc(RateCollection rates, string pair)
        {
            var separatePair = pair.SeparatePair();
            var currencyVsSymbol = rates.Rates.FirstOrDefault(x => x.Pair == $"{separatePair.currency}/{separatePair.symbol}");

            if (currencyVsSymbol == null)
                return null;

            return new QuoteOutput()
            {
                Symbol = separatePair.symbol,
                Currency = separatePair.currency,
                Price = 1 / currencyVsSymbol.Price,
                Timestamp = rates.Timestamp
            };
        }
    }
}