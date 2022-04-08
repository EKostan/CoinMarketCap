using System.Collections.Generic;
using Microsoft.Extensions.Options;

namespace CoinMarketCap.WebApi.Services.RateCalculators
{
    public class RateCalculatorRegistry
    {
        static Dictionary<string, IRateCalculator> _rateCalculators = new Dictionary<string, IRateCalculator>();

        public RateCalculatorRegistry()
        {
        }

        public static void AddCalculators(IEnumerable<string> pairs, IRateCalculator calculator)
        {
            foreach (var pair in pairs)
            {
                _rateCalculators[pair] = calculator;
            }
        }

        public static IRateCalculator GetCalculator(string pair)
        {
            if (!_rateCalculators.ContainsKey(pair))
            {
                return null;
            }

            return _rateCalculators[pair];
        }
    }
}