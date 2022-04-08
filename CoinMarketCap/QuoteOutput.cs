namespace CoinMarketCap
{
    public class QuoteOutput
    {
        public long Timestamp { get; set; }
        public decimal Price { get; set; }
        public string Symbol { get; set; }
        public string Currency { get; set; }
        public string Pair => $"{Symbol}/{Currency}";
    }
}