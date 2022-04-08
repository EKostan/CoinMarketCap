using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using CoinMarketCap.Extensions;
using Flurl;
using Flurl.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CoinMarketCap
{
    public class CoinMarketCapProvider
    {
        private readonly ILogger<CoinMarketCapProvider> _logger;
        private const int UpdateAterMinutes = 5;

        private readonly CoinMarketCapSettings _settings;
        private readonly Semaphore _semaphore = new Semaphore(1, 1);
        private DateTime? _lastUpdate;
        private RateCollection _lastRates;

        private bool NeedUpdate => _lastUpdate == null || _lastUpdate.Value.AddMinutes(UpdateAterMinutes) < DateTime.Now;

        public CoinMarketCapProvider(
            ILogger<CoinMarketCapProvider> logger,
            IOptions<CoinMarketCapSettings> settings)
        {
            _logger = logger;
            _settings = settings.Value;
        }

        public async Task<RateCollection> GetRates()
        {
            if (NeedUpdate)
            {
                await UpdateRates();
            }

            return _lastRates;
        }
        
        private async Task UpdateRates()
        {
            if (!_semaphore.WaitOne(0))
            {
                return;
            }

            try
            {
                var res = new RateCollection();
                foreach (var currency in _settings.Currency)
                {
                    var quotesOutput = await GetQuotes(_settings.SymbolsString, currency);
                    res.Rates.AddRange(quotesOutput.Rates);
                    // first timestamp
                    res.Timestamp = res.Timestamp == 0 ? quotesOutput.Timestamp : res.Timestamp;
                }

                _lastRates = res;
                _lastUpdate = DateTime.Now;
            }
            catch (Exception e)
            {
                _logger.LogError($"UpdateRates error: {e}");
            }
            finally
            {
                _semaphore.Release();
            }

        }

        public async Task<RateCollection> GetQuotes(string symbols, string currency)
        {
            var res = new RateCollection();

            var ids = ConvertToIds(symbols);
            var quotesOutput = await _settings.Url
                .AppendPathSegment("cryptocurrency/quotes/latest")
                .SetQueryParams(new
                {
                    CMC_PRO_API_KEY = _settings.GetApiKey(currency),
                    //symbol = symbols,
                    convert = currency,
                    id = ids
                })
                .GetJsonAsync<dynamic>();

            var timestamp = DateTime.Parse((string) quotesOutput.status.timestamp, CultureInfo.InvariantCulture).ToUnixTimeSeconds();
            res.Timestamp = timestamp;

            var data = quotesOutput.data;

            var symbolsList = symbols.Split(",", StringSplitOptions.RemoveEmptyEntries);

            foreach (var symbol in symbolsList)
            {
                dynamic item = ((object)data).GetProperty(GetSymbolId(symbol));

                var quoteOutput = new QuoteOutput
                {
                    Timestamp = timestamp,
                    Symbol = (string) item.symbol,
                    Currency = currency
                };

                dynamic quote = ((object)item.quote).GetProperty(currency);
                quoteOutput.Price = (decimal)quote.price;
                res.Rates.Add(quoteOutput);
            }

            return res;
        }

        

        private string ConvertToIds(string symbols)
        {
            var res = symbols;
            foreach (var info in _settings.SymbolInfos)
            {
                res = res.Replace(info.Symbol, info.Id.ToString());
            }
            return res;
        }

        private string GetSymbolId(string symbol)
        {
            foreach (var info in _settings.SymbolInfos)
            {
                if (info.Symbol == symbol)
                {
                    return info.Id.ToString();
                }
            }

            throw new Exception($"symbol {symbol} not support");
        }
    }
}
