using System.ComponentModel.DataAnnotations;

namespace Cryptaur.Burse.Contract.Model
{
    public class CloseOrderInput : PairInput
    {
        [Required]
        public long MarketOrderId { get; set; }
    }
}