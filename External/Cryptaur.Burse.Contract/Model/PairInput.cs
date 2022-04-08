using System.ComponentModel.DataAnnotations;
using Nordavind.Infrastructure.Models.Attributes;

namespace Cryptaur.Burse.Contract.Model
{
    public class PairInput
    {
        [Required]
        public long PairId { get; set; }

        [SwaggerExclude]
        public long MarketId { get; set; }
    }
}