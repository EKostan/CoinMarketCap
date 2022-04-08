namespace Cryptaur.Burse.Contract.Model
{
    public class GetDealHistoryInput : PairInput
    {
        public long FromDealId { get; set; }
        public bool UserHistory { get; set; }
    }
}