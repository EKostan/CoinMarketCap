using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoinMarketCap.WebApi.Services.RateCalculators.Burse
{
    public static class BaseCalculator
    {
        public static List<string> GetPairCombinations(IEnumerable<string> symbols, IEnumerable<string> currency)
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
    }

    public class ProofCalculator : IRateCalculator
    {
        private readonly ProofRatesContainer _pfRatesContainer;

        public ProofCalculator(CoinMarketCapSettings settings) 
        {
            _pfRatesContainer = new ProofRatesContainer(settings);
        }

        public static List<string> GetSupportedPairs(IEnumerable<string> symbols, IEnumerable<string> currency)
        {
            var res = new List<string>();
            res.AddRange(BaseCalculator.GetPairCombinations(new List<string>() { ProofRatesContainer.Proof }, currency));
            res.AddRange(BaseCalculator.GetPairCombinations(new List<string>() { ProofRatesContainer.Proof }, symbols));
            return res.Distinct().ToList();
        }


        public async Task<QuoteOutput> Calc(RateCollection rates, string pair)
        {
            var btcCalculator = new BtcCalculator();
            var proofRates = await _pfRatesContainer.GetQuoteOutputs(rates);

            return proofRates.Rates.FirstOrDefault(x => x.Pair == pair)
                   ?? await btcCalculator.Calc(proofRates, pair);
        }

    }

    public class PfCalculator : IRateCalculator
    {
        private readonly PfRatesContainer _pfRatesContainer;

        public PfCalculator(CoinMarketCapSettings settings)
        {
            _pfRatesContainer = new PfRatesContainer(settings);
        }

        public static List<string> GetSupportedPairs(IEnumerable<string> symbols, IEnumerable<string> currency)
        {
            var res = new List<string>();
            res.AddRange(BaseCalculator.GetPairCombinations(new List<string>() { PfRatesContainer.Pf }, currency));
            res.AddRange(BaseCalculator.GetPairCombinations(new List<string>() { PfRatesContainer.Pf }, symbols));
            return res.Distinct().ToList();
        }
        

        public async Task<QuoteOutput> Calc(RateCollection rates, string pair)
        {
            var btcCalculator = new BtcCalculator();
            var proofRates = await _pfRatesContainer.GetQuoteOutputs(rates);

            return proofRates.Rates.FirstOrDefault(x => x.Pair == pair)
                   ?? await btcCalculator.Calc(proofRates, pair);
        }
    }
}