using System.ComponentModel.DataAnnotations;

namespace Cryptaur.Burse.Contract.Model
{
    public class GetDealInput : PairInput
    {
        [Required]
        public long DealId { get; set; }
    }
}