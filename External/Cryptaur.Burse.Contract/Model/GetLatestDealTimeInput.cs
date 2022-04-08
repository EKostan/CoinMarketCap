using System.ComponentModel.DataAnnotations;

namespace Cryptaur.Burse.Contract.Model
{
    public class GetLatestDealTimeInput : PairInput
    {
        [Required]
        public long LatestDealTime { get; set; }
    }
}