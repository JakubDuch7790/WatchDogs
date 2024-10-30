using Microsoft.EntityFrameworkCore;
using Serilog;
using WatchDogs.Contracts;

namespace WatchDogs.Persistence.EntityFramework;
public class SuspiciousTradesInsertingUnitOfWork : IUnitOfWork
{
    private readonly SuspiciousTradesDbContext _context;
    private readonly ILogger _logger;

    public IDataInserter DataInserter { get; }

    public SuspiciousTradesInsertingUnitOfWork(DbContextOptions<SuspiciousTradesDbContext> options, ILogger logger)
    {
        _context = new SuspiciousTradesDbContext(options);
        DataInserter = new SuspiciousTradesInserter(_context, logger);
    }

    public async Task SaveAsync()
    {
        await _context.SaveChangesAsync();
    }


    public void Dispose()
    {
        _context.Dispose();
    }

}
