using System.ComponentModel.DataAnnotations;
using Nordavind.Infrastructure.Models.Attributes;

namespace Cryptaur.Burse.Contract.Model
{
    public class GetBarsInput : PairInput
    {
        [Required]
        [PossibleValues(typeof(TimeFrames))]
        public string TimeFrame { get; set; }

        [PossibleValues(typeof(TimeIntervals))]
        public string TimeInterval { get; set; }
    }
}