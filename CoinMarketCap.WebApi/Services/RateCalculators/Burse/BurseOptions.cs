using Cryptaur.Burse.Contract;
using Microsoft.Extensions.Options;

namespace CoinMarketCap.WebApi.Services.RateCalculators.Burse
{
    public class BurseOptions : IOptions<BurseApiSettings>
    {
        public BurseOptions(BurseApiSettings settings)
        {
            Value = settings;
        }

        public BurseApiSettings Value { get; }
    }
}