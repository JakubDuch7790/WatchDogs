using Microsoft.EntityFrameworkCore;
using WatchDogs.Contracts;

namespace WatchDogs.Persistence.EntityFramework;
public class SuspiciousTradesDbContext : DbContext
{
    public SuspiciousTradesDbContext(DbContextOptions<SuspiciousTradesDbContext> options) : base(options)
    {
            
    }

    public DbSet<Trade> SuspiciousTrades { get; set; }
}
