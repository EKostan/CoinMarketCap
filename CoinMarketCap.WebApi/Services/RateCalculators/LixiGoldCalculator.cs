using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoinMarketCap.WebApi.Services.RateCalculators
{
    public class LixiGoldCalculator : IRateCalculator
    {
        public const decimal LIXIGOLD_USD = 1.0m;
        LixiGoldRatesContainer _ratesContainer = new LixiGoldRatesContainer();

        public static List<string> GetSupportedPairs(IEnumerable<string> symbols, IEnumerable<string> currency)
        {
            var res = new List<string>();
            res.AddRange(GetPairCombinations(new List<string>() { "LIXIGOLD" }, currency));
            res.AddRange(GetPairCombinations(new List<string>() { "LIXIGOLD" }, symbols));
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
            var proofRates = await _ratesContainer.GetQuoteOutputs(rates);

            return proofRates.Rates.FirstOrDefault(x => x.Pair == pair)
                   ??  await btcCalculator.Calc(proofRates, pair);
        }
    }

    public class LixiGoldRatesContainer
    {
        public const string LIXIGOLD = "LIXIGOLD";
        public const string USD = "USD";
        public const string EUR = "EUR";
        public const string RUB = "RUB";
        public const string BTC = "BTC";
        public const string VND = "VND";

        private RateCollection _rates;
        private RateCollection _lixiGoldRates;

        public async Task<RateCollection> GetQuoteOutputs(RateCollection rates)
        {
            if (_rates != rates || _lixiGoldRates == null)
            {
                _rates = rates;
                await CalculateRates(_rates);
            }

            return _lixiGoldRates;
        }

        private async Task CalculateRates(RateCollection rates)
        {
            _lixiGoldRates = new RateCollection()
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

            var proofUsd = LixiGoldCalculator.LIXIGOLD_USD;

            _lixiGoldRates.Rates.AddRange(new List<QuoteOutput>
            {
                CreateQuote(LIXIGOLD, USD, proofUsd, timestamp),
                CreateQuote(USD, LIXIGOLD, 1 / proofUsd, timestamp),

                CreateQuote(LIXIGOLD, EUR, proofUsd / eurUsd.Price, timestamp),
                CreateQuote(EUR, LIXIGOLD, usdEur.Price / proofUsd, timestamp),

                CreateQuote(LIXIGOLD, RUB, proofUsd / rubUsd.Price, timestamp),
                CreateQuote(RUB, LIXIGOLD, rubUsd.Price / proofUsd, timestamp),

                CreateQuote(LIXIGOLD, VND, proofUsd / vndUsd.Price , timestamp),
                CreateQuote(VND, LIXIGOLD, vndUsd.Price / proofUsd, timestamp),

                CreateQuote(LIXIGOLD, BTC, usdBtc.Price * proofUsd, timestamp),
                CreateQuote(BTC, LIXIGOLD, btcUsd.Price / proofUsd, timestamp)
            });
        }

        private QuoteOutput CreateQuote(string symbol, string currency, decimal price, long timestamp)
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
