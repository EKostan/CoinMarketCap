using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Cryptaur.Burse.Contract;
using Cryptaur.Burse.Contract.Model;

namespace CoinMarketCap.WebApi.Services.RateCalculators.Burse
{
    public abstract class BurseRatesContainer
    {
        protected readonly CoinMarketCapSettings _settings;
        protected readonly BurseApi _burseApi;
        public const string USD = "USD";
        public const string EUR = "EUR";
        public const string RUB = "RUB";
        public const string ETH = "ETH";
        public const string BTC = "BTC";
        public const string VND = "VND";


        protected RateCollection _rates;
        protected RateCollection _pfRates;

        protected abstract string Ticker { get; }
        protected abstract int TickerPairId { get; }

        protected BurseRatesContainer(CoinMarketCapSettings settings)
        {
            _settings = settings;
            _burseApi = new BurseApi(new BurseOptions(settings.BurseApiSettings));
        }

        public async Task<RateCollection> GetQuoteOutputs(RateCollection rates)
        {
            if (_rates != rates || _pfRates == null)
            {
                _rates = rates;
                await CalculateProofRates(_rates);
            }

            return _pfRates;
        }

        protected virtual async Task CalculateProofRates(RateCollection rates)
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

            var stat24HResult = await _burseApi.GetStat24H(new GetStat24HInput()
            {
                PairId = TickerPairId
            });

            var pfEth = CreateQuote(Ticker, ETH, ToUnit(stat24HResult.Stat24H.Close), timestamp);
            var pfUsd = pfEth.Price * ethUsd.Price;

            _pfRates.Rates.AddRange(new List<QuoteOutput>
            {
                CreateQuote(Ticker, USD, pfUsd, timestamp),
                CreateQuote(USD, Ticker, 1 / pfUsd, timestamp),

                CreateQuote(Ticker, EUR, pfUsd / eurUsd.Price, timestamp),
                CreateQuote(EUR, Ticker, usdEur.Price / pfUsd, timestamp),

                CreateQuote(Ticker, RUB, pfUsd / rubUsd.Price, timestamp),
                CreateQuote(RUB, Ticker, rubUsd.Price / pfUsd, timestamp),

                CreateQuote(Ticker, VND, pfUsd / vndUsd.Price , timestamp),
                CreateQuote(VND, Ticker, vndUsd.Price / pfUsd, timestamp),

                CreateQuote(Ticker, BTC, usdBtc.Price * pfUsd, timestamp),
                CreateQuote(BTC, Ticker, btcUsd.Price / pfUsd, timestamp),

                pfEth,
                CreateQuote(ETH, Ticker, 1/pfEth.Price, timestamp)
            });
        }

        protected virtual decimal ToUnit(ulong atomicUnits)
        {
            return (decimal) atomicUnits / (decimal) BigInteger.Pow(10, _settings.EthDecimals);
        }

        protected QuoteOutput CreateQuote(string symbol, string currency, decimal price, long timestamp)
        {
            return new QuoteOutput()
            {
                Price = price,
                Symbol = symbol,
                Currency = currency,
                Timestamp = timestamp
            };
        }
    }
}