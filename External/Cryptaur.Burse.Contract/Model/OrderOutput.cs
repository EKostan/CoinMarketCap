namespace Cryptaur.Burse.Contract.Model
{
    public class OrderOutput 
    {
        public long OrderId { get; set; }

        public long? MarketOrderId { get; set; }

        public string InvestorId { get; set; }

        public string Status { get; set; }
    }
}