using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace CoinMarketCap.Dal
{
    public interface ICoinMarketCapContext
    {
        DbSet<QuoteHistory> QuoteHistories { get; set; }
        DbSet<Quote> Quotes { get; set; }
        DbSet<LogEntry> LogEntries { get; set; }
        DbSet<App> Apps { get; set; }
        int SaveChanges();
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken));
    }
}