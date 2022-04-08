using Newtonsoft.Json;

namespace Cryptaur.Burse.Contract.Model
{
    public class Stat24HResult
    {
        [JsonProperty("stat_24h")]
        public Stat24H Stat24H { get; set; }

        public bool GotChanges(Stat24HResult stat24HResult)
        {
            if (Stat24H == null)
            {
                return true;
            }

            if (stat24HResult?.Stat24H == null)
            {
                return false;
            }

            return Stat24H.GotChanges(stat24HResult.Stat24H);
        }
    }
}