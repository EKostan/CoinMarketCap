using System.Threading.Tasks;

namespace CoinMarketCap.WebApi.Services.RateCalculators
{
    public interface IRateCalculator
    {
        Task<QuoteOutput> Calc(RateCollection rates, string pair);
    }
}