using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoinMarketCap.WebApi.Services.RateCalculators.Burse
{
    public class LixiCalculator : IRateCalculator
    {
        private readonly LixiRatesContainer _lixiRatesContainer;

        public LixiCalculator(CoinMarketCapSettings settings)
        {
            _lixiRatesContainer = new LixiRatesContainer(settings);
        }

        public static List<string> GetSupportedPairs(IEnumerable<string> symbols, IEnumerable<string> currency)
        {
            var res = new List<string>();
            res.AddRange(GetPairCombinations(new List<string>() { "LIXI" }, currency));
            res.AddRange(GetPairCombinations(new List<string>() { "LIXI" }, symbols));
            return res.Distinct().ToList();
        }

        private static List<string> GetPairCombinations(IEnumerable<string> symbols, IEnumerable<string> currency)
        {
            var res = new List<string>();

            foreach (var symbol in symbols)
            {
                foreach (var cur in currency)
                {
                    res.Add($"{symbol}/{cur}");
                    res.Add($"{cur}/{symbol}");
                }
            }

            return res;
        }

        public async Task<QuoteOutput> Calc(RateCollection rates, string pair)
        {
            var btcCalculator = new BtcCalculator();
            var proofRates = await _lixiRatesContainer.GetQuoteOutputs(rates);

            return proofRates.Rates.FirstOrDefault(x => x.Pair == pair)
                   ?? await btcCalculator.Calc(proofRates, pair);
        }
    }

    
}