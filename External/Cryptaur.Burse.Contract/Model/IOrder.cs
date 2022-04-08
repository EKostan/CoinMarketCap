namespace Cryptaur.Burse.Contract.Model
{
    public interface IOrder
    {
        string OrderType { get; set; }

        ulong Price { get; set; }

        ulong Amount { get; set; }
    }
}