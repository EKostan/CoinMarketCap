using System.ComponentModel.DataAnnotations;
using Nordavind.Infrastructure.Models.Attributes;

namespace Cryptaur.Burse.Contract.Model
{
    public class PlaceOrderInput : PairInput, IOrder
    {
        [Required]
        [PossibleValues(typeof(OrderTypes))]
        public string OrderType { get; set; }

        [Required]
        public ulong Price { get; set; }

        [Required]
        public ulong Amount { get; set; }
    }
}