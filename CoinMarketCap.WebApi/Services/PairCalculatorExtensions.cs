using System.Collections.Generic;
using System.IO;
using System.Text;
using CoinMarketCap.WebApi.Services.RateCalculators;
using CoinMarketCap.WebApi.Services.RateCalculators.Burse;
using Microsoft.Extensions.DependencyInjection;

namespace CoinMarketCap.WebApi.Services
{
    public static class PairCalculatorExtensions
    {
        public static void AddPairCalculators(this IServiceCollection services, CoinMarketCapSettings settings)
        {
            services.AddSingleton<RateCalculatorRegistry>();
            RateCalculatorRegistry.AddCalculators(BtcCalculator.GetSupportedPairs(settings.Symbols), new BtcCalculator());
            RateCalculatorRegistry.AddCalculators(RevertCurrencyCalculator.GetSupportedPairs(settings.Symbols, settings.Currency), new RevertCurrencyCalculator());
            RateCalculatorRegistry.AddCalculators(CurrencyCalculator.GetSupportedPairs(settings.Currency), new CurrencyCalculator());
            RateCalculatorRegistry.AddCalculators(ProofCalculator.GetSupportedPairs(settings.Symbols, settings.Currency), new ProofCalculator(settings));
            RateCalculatorRegistry.AddCalculators(PfCalculator.GetSupportedPairs(settings.Symbols, settings.Currency), new PfCalculator(settings));
            RateCalculatorRegistry.AddCalculators(LixiCalculator.GetSupportedPairs(settings.Symbols, settings.Currency), new LixiCalculator(settings));
            RateCalculatorRegistry.AddCalculators(LixiGoldCalculator.GetSupportedPairs(settings.Symbols, settings.Currency), new LixiGoldCalculator());
        }
    }
}