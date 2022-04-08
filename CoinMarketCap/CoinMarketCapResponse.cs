namespace CoinMarketCap
{
    public class CoinMarketCapResponse
    {
        public DataObject Data { get; set; }

        public class DataObject
        {
            public QuotesObject Quotes { get; set; }

            public class QuotesObject
            {
                public Currency Usd { get; set; }

                public Currency Eur { get; set; }

                public Currency Rub { get; set; }

                public Currency Btc { get; set; }


                public class Currency
                {
                    public decimal Price { get; set; }
                }
            }
        }
    }
}
