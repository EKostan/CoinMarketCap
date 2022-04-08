using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using CoinMarketCap.Dal;
using CoinMarketCap.Extensions;
using CoinMarketCap.WebApi.Services.RateCalculators;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace CoinMarketCap.WebApi.Services
{
    public class CoinMarketCapManager
    {
        private readonly CoinMarketCapProvider _coinMarketCapProvider;
        private readonly ICoinMarketCapContext _context;
        private readonly CoinMarketCapSettings _settings;
        private readonly ILogger<CoinMarketCapManager> _logger;

        static CoinMarketCapManager()
        {
            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<Quote, QuoteHistory>().ForMember(x => x.Id, y => y.Ignore());
                cfg.CreateMap<QuoteHistory, Quote>().ForMember(x => x.Id, y => y.Ignore());
            });
        }

        public CoinMarketCapManager(
            CoinMarketCapProvider coinMarketCapProvider,
            ICoinMarketCapContext context,
            IOptions<CoinMarketCapSettings> settings,
            ILogger<CoinMarketCapManager> logger)
        {
            _coinMarketCapProvider = coinMarketCapProvider;
            _context = context;
            _settings = settings.Value;
            _logger = logger;
        }

        public async Task UpdateQuotes()
        {
            var symbols = new List<string>();
            symbols.AddRange(_settings.Symbols);
            symbols.AddRange(_settings.Currency);
            symbols.AddRange(_settings.CustomSymbols);

            var combinations = new HashSet<string>(GetCombinations(symbols));

            var rateCollection = await _coinMarketCapProvider.GetRates();

            //var sb = new StringBuilder();
            //foreach (var pair in combinations)
            //{
            //    sb.AppendLine($"{pair}");
            //}

            //File.WriteAllText(
            //        "D:\\Nordawind\\cryptaur-cpt-exchange\\api\\CoinMarketCap\\CoinMarketCap.WebApi.Tests\\Combinations.csv",
            //        sb.ToString());

            //var sb2 = new StringBuilder();
            //foreach (var rate in rateCollection.Rates)
            //{
            //    sb2.AppendLine($"{rate.Pair};{rate.Price};{rate.Symbol};{rate.Currency}");
            //}

            //File.WriteAllText(
            //    "D:\\Nordawind\\cryptaur-cpt-exchange\\api\\CoinMarketCap\\CoinMarketCap.WebApi.Tests\\RateCollection.csv",
            //    sb2.ToString());

            //File.WriteAllText("D:\\Nordawind\\cryptaur-cpt-exchange\\api\\CoinMarketCap\\CoinMarketCap.WebApi.Tests\\RateCollection.json", JsonConvert.SerializeObject(rateCollection));

            var quotes = _context.Quotes.Where(x => combinations.Contains(x.Pair)).ToList();
            var calcInfo = await Calculate(rateCollection, quotes, combinations);
            _context.Quotes.AddRange(calcInfo.InsertQuotes);
            _context.QuoteHistories.AddRange(calcInfo.QuotesHistory);
            _context.SaveChanges();
        }

        public async Task<CalcInfo> Calculate(
            RateCollection rateCollection, 
            List<Quote> quotes, 
            HashSet<string> combinations)
        {
            var res = new CalcInfo();

            foreach (var currencyPair in combinations)
            {
                var quote = quotes.FirstOrDefault(x => x.Pair == currencyPair);

                if (quote == null)
                {
                    var separatePair = currencyPair.SeparatePair();
                    quote = new Quote()
                    {
                        Symbol = separatePair.symbol,
                        Currency = separatePair.currency,
                        Pair = currencyPair,
                        Timestamp = rateCollection.Timestamp
                    };
                    res.InsertQuotes.Add(quote);
                }

                var rate = rateCollection.Rates.FirstOrDefault(x => x.Pair == currencyPair);

                if (rate == null)
                {
                    rate = await CalculateCustomRate(rateCollection, currencyPair);

                    if (rate == null)
                        continue;
                }

                quote.Price = rate.Price;
                quote.Timestamp = rateCollection.Timestamp;

                res.Quotes.Add(quote);
                res.QuotesHistory.Add(Mapper.Map<QuoteHistory>(quote));
            }

            return res;
        }

        public class CalcInfo
        {
            public List<Quote> Quotes { get; set; } = new List<Quote>();
            public List<Quote> InsertQuotes { get; set; } = new List<Quote>();
            public List<QuoteHistory> QuotesHistory { get; set; } = new List<QuoteHistory>();
        }

        private async Task<QuoteOutput> CalculateCustomRate(RateCollection rates, string pair)
        {
            var rateCalculator = RateCalculatorRegistry.GetCalculator(pair);
            
            if (rateCalculator == null)
            {
                return null;
            }

            return await rateCalculator?.Calc(rates, pair);
        }

        private (string symbol, string currency) SeparatePair(string pair)
        {
            var s = pair.Split("/", StringSplitOptions.RemoveEmptyEntries);
            return (s[0], s[1]);
        }

        public static List<string> GetCombinations(IEnumerable<string> symbols)
        {
            var res = new List<string>();
            var permutations = GetPermutations(symbols, 2);

            foreach (var permutation in permutations)
            {
                var list = permutation.ToList();
                res.Add($"{list[0]}/{list[1]}");
            }

            return res.Distinct().ToList();
        }

        static IEnumerable<IEnumerable<T>>
            GetPermutations<T>(IEnumerable<T> list, int length)
        {
            if (length == 1) return list.Select(t => new T[] { t });
            return GetPermutations(list, length - 1)
                .SelectMany(t => list.Where(o => !t.Contains(o)),
                    (t1, t2) => t1.Concat(new T[] { t2 }));
        }

        public IQuote GetQuote(string pair, long timestamp)
        {
            IQuote quote = _context.Quotes.FirstOrDefault(x => x.Pair == pair);

            if (timestamp != 0)
            {
                var quoteHistory = _context.QuoteHistories
                    .OrderByDescending(x => x.Timestamp)
                    .Where(x => x.Timestamp <= timestamp)
                    .FirstOrDefault(x => x.Pair == pair);

                quote = quoteHistory ?? throw new Exception($"Early timestamp not found");
            }

            if (quote == null)
            {
                throw new Exception($"Pair {pair} not found");
            }

            return quote;
        }

        public List<IQuote> GetQuotes(string symbols, string currency, long timestamp)
        {
            var res = new List<IQuote>();
            return GetQuotes(GetSymbolsCurrencyPairs(symbols, currency), timestamp);
        }

        public List<IQuote> GetQuotes(IEnumerable<string> pairs, long timestamp)
        {
            var res = new List<IQuote>();
            var hsPairs = new HashSet<string>(pairs);

            if (timestamp == 0)
            {
                var quotes = _context.Quotes.Where(x => hsPairs.Contains(x.Pair)).ToList();
                return quotes.Select(x => (IQuote)x).ToList();
            }

            var day = TimeSpan.FromDays(1).TotalSeconds;

            var quotesHistory = _context.QuoteHistories
                .Where(x => x.Timestamp <= timestamp && x.Timestamp > (timestamp - day))
                .GroupBy(y => y.Pair)
                .ToDictionary(x => x.Key, y => y.OrderByDescending(o => o.Timestamp).ToList());


            if (quotesHistory.Count <= 0)
            {
                quotesHistory = FindCloseQuoteHistories(timestamp);
            }

            foreach (var pair in hsPairs)
            {
                if (!quotesHistory.ContainsKey(pair))
                {
                    throw new Exception($"Pair {pair} not found");
                }

                var quote = quotesHistory[pair].FirstOrDefault();

                if (quote == null)
                {
                    throw new Exception($"Pair {pair} not found");
                }

                res.Add(quote);
            }

            return res;
        }

        private Dictionary<string, List<QuoteHistory>> FindCloseQuoteHistories(long timestamp)
        {
            var quotesHistory = new Dictionary<string, List<QuoteHistory>>();
            var leftTime = _context.QuoteHistories.FirstOrDefault(x => x.Timestamp < timestamp)?.Timestamp;
            var rightTime = _context.QuoteHistories.FirstOrDefault(x => x.Timestamp > timestamp)?.Timestamp;

            if (leftTime == null && rightTime == null)
            {
                return quotesHistory;
            }

            long leftDelta = long.MaxValue;
            long rightDelta = long.MaxValue;
            if (leftTime != null)
            {
                leftDelta = timestamp - leftTime.Value;
            }

            if (rightTime != null)
            {
                rightDelta = rightTime.Value - timestamp;
            }

            var newTimestamp = leftDelta < rightDelta ? leftTime.Value : rightTime.Value;

            quotesHistory = _context.QuoteHistories
                .Where(x => x.Timestamp == newTimestamp)
                .GroupBy(y => y.Pair)
                .ToDictionary(x => x.Key, y => y.OrderByDescending(o => o.Timestamp).ToList());

            return quotesHistory;
        }

        private List<string> GetSymbolsCurrencyPairs(string symbols, string currency)
        {
            var res = new List<string>();
            var symbolList = new List<string>(symbols.ToUpper().Split(",", StringSplitOptions.RemoveEmptyEntries));
            var currencyList = new List<string>(currency.ToUpper().Split(",", StringSplitOptions.RemoveEmptyEntries));

            foreach (var symbol in symbolList)
            {
                foreach (var cur in currencyList)
                {
                    res.Add($"{symbol}/{cur}");
                }
            }

            return res;
        }
    }
}