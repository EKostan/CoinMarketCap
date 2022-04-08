namespace CoinMarketCap.WebApi.Services.RateCalculators.Burse
{
    public class PfRatesContainer : BurseRatesContainer
    {
        public const string Pf = "PF";
        public const string Proof = "PROOF";

        public PfRatesContainer(CoinMarketCapSettings settings) 
            : base(settings)
        {
        }

        protected override string Ticker => Pf;
        protected override int TickerPairId => _settings.PfEthPairId;
    }    
    
    public class ProofRatesContainer : BurseRatesContainer
    {
        public const string Proof = "PROOF";

        public ProofRatesContainer(CoinMarketCapSettings settings) 
            : base(settings)
        {
        }

        protected override string Ticker => Proof;
        protected override int TickerPairId => _settings.PfEthPairId;
    }
}