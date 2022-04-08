using System;

namespace CoinMarketCap.WebApi.Services
{
    public static class StringExtension
    {
        public static (string symbol, string currency) SeparatePair(this string pair)
        {
            var s = pair.Split("/", StringSplitOptions.RemoveEmptyEntries);
            return (s[0], s[1]);
        }
    }
}