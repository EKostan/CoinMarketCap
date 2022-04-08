using System.Collections.Generic;

namespace CoinMarketCap
{
    public class RateCollection
    {
        public long Timestamp { get; set; }
        public List<QuoteOutput> Rates { get; set; } = new List<QuoteOutput>();
    }
}