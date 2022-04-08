using Microsoft.EntityFrameworkCore;

namespace CoinMarketCap.Dal
{
    public class CoinMarketCapContext : DbContext, ICoinMarketCapContext
    {
        public CoinMarketCapContext(DbContextOptions<CoinMarketCapContext> options)
            : base(options)
        {
        }


        public virtual DbSet<QuoteHistory> QuoteHistories { get; set; }

        /// <summary>
        /// Actual quotes
        /// </summary>
        public virtual DbSet<Quote> Quotes { get; set; }


        public virtual DbSet<LogEntry> LogEntries { get; set; }

        public virtual DbSet<App> Apps { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<QuoteHistory>()
                .HasIndex(b => b.Timestamp);

            base.OnModelCreating(modelBuilder);
        }
    }
}
