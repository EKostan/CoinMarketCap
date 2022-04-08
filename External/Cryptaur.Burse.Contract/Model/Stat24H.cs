using System.Numerics;
using Newtonsoft.Json;

namespace Cryptaur.Burse.Contract.Model
{
    public class Stat24H
    {
        [JsonProperty("close")]
        public ulong Close { get; set; }

        [JsonProperty("high")]
        public ulong High { get; set; }

        [JsonProperty("low")]
        public ulong Low { get; set; }

        [JsonProperty("money_volume")]
        public BigInteger MoneyVolume { get; set; }

        [JsonProperty("open")]
        public ulong Open { get; set; }

        [JsonProperty("start_time")]
        public ulong StartTime { get; set; }

        [JsonProperty("volume")]
        public ulong Volume { get; set; }

        public decimal ChangePercent { get; set; }

        public bool GotChanges(Stat24H stat24H)
        {
            var res = false;

            res |= Close != stat24H.Close;
            res |= High != stat24H.High;
            res |= Low != stat24H.Low;
            res |= MoneyVolume != stat24H.MoneyVolume;
            res |= Open != stat24H.Open;
            res |= StartTime != stat24H.StartTime;
            res |= StartTime != stat24H.StartTime;
            res |= Volume != stat24H.Volume;

            return res;
        }

        public void CalcStat()
        {
            if (Open == 0)
            {
                ChangePercent = 0;
                return;
            }

            ChangePercent =  (decimal)Close / (decimal)Open * 100 - 100;
        }
    }
}