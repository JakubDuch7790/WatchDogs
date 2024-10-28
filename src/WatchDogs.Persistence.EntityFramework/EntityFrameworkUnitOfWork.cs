using Microsoft.EntityFrameworkCore;
using WatchDogs.Contracts;

namespace WatchDogs.Persistence.EntityFramework;

public class EntityFrameworkUnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    
    public ITradeInserter DataInserter { get; }

    public EntityFrameworkUnitOfWork(DbContextOptions<ApplicationDbContext> options)
    {
        _context = new ApplicationDbContext(options);
        DataInserter = new TradeInserter(_context);
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
