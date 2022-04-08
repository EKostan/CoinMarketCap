using System.ComponentModel.DataAnnotations;
using Nordavind.Infrastructure.Models.Attributes;

namespace Cryptaur.Burse.Contract.Model
{
    public class GetCurrentBarInput : PairInput
    {
        [Required]
        [PossibleValues(typeof(TimeFrames))]
        public string TimeFrame { get; set; }
    }
}