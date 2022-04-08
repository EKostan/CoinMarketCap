using System;
using CoinMarketCap.Extensions;
using Newtonsoft.Json;

namespace CoinMarketCap.WebApi.Controllers
{
    internal class ErrorOutput
    {
        [JsonProperty("error")]
        public string Error { get; set; }

        [JsonProperty("timespan")]
        public long TimeSpan { get; set; }

        [JsonProperty("errorCode")]
        public int ErrorCode { get; set; } = 400;

        public static ErrorOutput CreateError(string error)
        {
            return new ErrorOutput()
            {
                TimeSpan = DateTime.UtcNow.ToUnixTimeSeconds(),
                Error = error
            };
        }
    }
}