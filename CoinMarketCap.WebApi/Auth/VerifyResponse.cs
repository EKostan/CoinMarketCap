namespace CoinMarketCap.WebApi.Auth
{
    public class VerifyResponse
    {
        public bool IsExist { get; set; }

        public bool IsLoginExist { get; set; }

        public bool KeyValid { get; set; }
    }
}