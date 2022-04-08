using System.Linq;
using Microsoft.Extensions.Options;

namespace CoinMarketCap.Tests
{
    public class CoinMarketCapOption : IOptions<CoinMarketCapSettings>
    {
        public CoinMarketCapSettings Value => new CoinMarketCapSettings()
        {
            Symbols = new string[] {"ETH",
                "XEM",
                "BTC",
                "DOGE",
                "ETC",
                "BTG",
                "BCH",
                "CPT"},
            Currency = new[] {"BTC",
                "USD",
                "EUR",
                "RUB"},
            Url = "https://pro-api.coinmarketcap.com/v1",
            ApiKeyBtc = "10975b7d-9925-4032-bce1-bc8af1ed610d",
            ApiKeyRub = "86789d9c-72e9-4a38-839e-7fc0aaa27e2e",
            ApiKeyEur = "702913cd-4d72-454b-81e6-1fc7f420cfd2",
            ApiKeyUsd = "942650b5-c8d5-4d7d-8b47-ff818d5cfcfb"
        };
    }


}
