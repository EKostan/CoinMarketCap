using Cryptaur.Burse.Contract;

namespace CoinMarketCap
{
    public class CoinMarketCapSettings
    {
        public string Url { get; set; }
        public string[] Symbols { get; set; }
        public string[] Currency { get; set; }
        public string[] CustomSymbols { get; set; }
        public SymbolInfo[] SymbolInfos { get; set; }
        public string ApiKeyBtc { get; set; }
        public string ApiKeyUsd { get; set; }
        public string ApiKeyEur { get; set; }
        public string ApiKeyRub { get; set; }
        public string ApiKeyVnd { get; set; }
        public int EthDecimals { get; set; }
        public int LixiDecimals { get; set; }
        public int LixiEthPairId { get; set; }
        public int LixiUsdtPairId { get; set; }
        public int PfEthPairId { get; set; }


        public BurseApiSettings BurseApiSettings { get; set; }

        public string SymbolsString => string.Join(",", Symbols);

        public string GetApiKey(string currency)
        {
            switch (currency.ToUpper())
            {
                case "BTC":
                    return ApiKeyBtc;
                case "USD":
                    return ApiKeyUsd;
                case "EUR":
                    return ApiKeyEur;
                case "RUB":
                    return ApiKeyRub;
                case "VND":
                    return ApiKeyVnd;
                default:
                    return string.Empty;
            }
        }
    }

    public class SymbolInfo
    {
        public string Symbol { get; set; }
        public int Id { get; set; }
    }
}
