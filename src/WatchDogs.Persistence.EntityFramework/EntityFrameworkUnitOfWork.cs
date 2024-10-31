using Microsoft.EntityFrameworkCore;
using Serilog;
using WatchDogs.Contracts;

namespace WatchDogs.Persistence.EntityFramework;

public class EntityFrameworkUnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger _logger;
    
    public IDataInserter DataInserter { get; }

    public EntityFrameworkUnitOfWork(DbContextOptions<ApplicationDbContext> options, ILogger logger)
    {
        _context = new ApplicationDbContext(options);
        DataInserter = new TradeInserter(_context, logger);
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
