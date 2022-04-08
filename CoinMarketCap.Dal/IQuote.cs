namespace CoinMarketCap.Dal
{
    public interface IQuote
    {
        long Timestamp { get; set; }
        decimal Price { get; set; }
        string Symbol { get; set; }
        string Currency { get; set; }
        string Pair { get; set; }
    }
}