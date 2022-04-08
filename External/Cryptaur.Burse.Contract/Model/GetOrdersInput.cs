namespace Cryptaur.Burse.Contract.Model
{
    public class GetOrdersInput : PairInput
    {
        public int SnapshotId { get; set; }
        public int FromChangeId { get; set; }

    }
}