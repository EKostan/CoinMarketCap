using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Cryptaur.Burse.Contract;
using Cryptaur.Burse.Contract.Model;

namespace CoinMarketCap.WebApi.Services.RateCalculators.Burse
{

    public class LixiRatesContainer : BurseRatesContainer
    {
        public LixiRatesContainer(CoinMarketCapSettings settings)
            : base(settings)
        {
        }

        protected override string Ticker => "LIXI";
        protected override int TickerPairId => _settings.LixiEthPairId;

        protected override async Task CalculateProofRates(RateCollection rates)
        {
            _pfRates = new RateCollection()
            {
                Rates = new List<QuoteOutput>(rates.Rates),
                Timestamp = rates.Timestamp
            };

            var currencyCalculator = new CurrencyCalculator();
            var timestamp = rates.Timestamp;
            var eurUsd = await currencyCalculator.Calc(rates, "EUR/USD");
            var usdEur = await currencyCalculator.Calc(rates, "USD/EUR");
            var rubUsd = await currencyCalculator.Calc(rates, "RUB/USD");
            var vndUsd = await currencyCalculator.Calc(rates, "VND/USD");

            var usdBtc = await currencyCalculator.Calc(rates, "USD/BTC");
            var btcUsd = rates.Rates.FirstOrDefault(x => x.Pair == "BTC/USD");

            var ethUsd = rates.Rates.FirstOrDefault(x => x.Pair == "ETH/USD");

            var lixiUsdtStat24HResult = await _burseApi.GetStat24H(new GetStat24HInput()
            {
                PairId = _settings.LixiUsdtPairId
            });

            var lixiUsdt = CreateQuote(Ticker, USD, ToUnit(lixiUsdtStat24HResult.Stat24H.Close), timestamp);
            var lixiUsd = lixiUsdt.Price;

            _pfRates.Rates.AddRange(new List<QuoteOutput>
            {
                CreateQuote(Ticker, USD, lixiUsd, timestamp),
                CreateQuote(USD, Ticker, 1 / lixiUsd, timestamp),

                CreateQuote(Ticker, EUR, lixiUsd / eurUsd.Price, timestamp),
                CreateQuote(EUR, Ticker, usdEur.Price / lixiUsd, timestamp),

                CreateQuote(Ticker, RUB, lixiUsd / rubUsd.Price, timestamp),
                CreateQuote(RUB, Ticker, rubUsd.Price / lixiUsd, timestamp),

                CreateQuote(Ticker, VND, lixiUsd / vndUsd.Price , timestamp),
                CreateQuote(VND, Ticker, vndUsd.Price / lixiUsd, timestamp),

                CreateQuote(Ticker, BTC, usdBtc.Price * lixiUsd, timestamp),
                CreateQuote(BTC, Ticker, btcUsd.Price / lixiUsd, timestamp),

                CreateQuote(Ticker, ETH,  lixiUsd / ethUsd.Price, timestamp),
                CreateQuote(ETH, Ticker, ethUsd.Price / lixiUsd, timestamp),

            });
        }

        protected override decimal ToUnit(ulong atomicUnits)
        {
            return (decimal)atomicUnits / (decimal)BigInteger.Pow(10, _settings.LixiDecimals);
        }
    }
}