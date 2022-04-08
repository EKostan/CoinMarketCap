using System;

namespace CoinMarketCap.Extensions
{
    public static class DateExtensions
    {
        public static long ToUnixTimeMilliseconds(this DateTime dateTime) =>
            (long)(dateTime - DateTime.UnixEpoch).TotalMilliseconds;

        public static long ToUnixTimeSeconds(this DateTime dateTime) =>
            (long)(dateTime - DateTime.UnixEpoch).TotalSeconds;

        public static DateTime FromUnixTimeMilliseconds(this long unixTimestamp) =>
            DateTimeOffset.FromUnixTimeMilliseconds(unixTimestamp).UtcDateTime;
    }
}