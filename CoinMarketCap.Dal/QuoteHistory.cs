using System.ComponentModel.DataAnnotations.Schema;

namespace CoinMarketCap.Dal
{
    public class QuoteHistory : IQuote
    {
        public long Id { get; set; }

        public long Timestamp { get; set; }

        public decimal Price { get; set; }

        [Column(TypeName = "varchar(10)")]
        public string Symbol { get; set; }

        [Column(TypeName = "varchar(10)")]
        public string Currency { get; set; }

        [Column(TypeName = "varchar(255)")]
        public string Pair { get; set; }

    }
}