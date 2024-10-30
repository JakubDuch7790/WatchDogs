using Microsoft.EntityFrameworkCore;
using Serilog;
using WatchDogs.Contracts;

namespace WatchDogs.Persistence.EntityFramework;
public class SuspiciousTradesInsertingUnitOfWorkFactory : IUnitOfWorkFactory
{
    private readonly DbContextOptions<SuspiciousTradesDbContext> _dbContextOptions;
    private readonly ILogger _logger;

    public SuspiciousTradesInsertingUnitOfWorkFactory(DbContextOptions<SuspiciousTradesDbContext> dbContextOptions, ILogger logger)
    {
        _dbContextOptions = dbContextOptions;
        _logger = logger;
    }

    public IUnitOfWork Create()
    {
        return new SuspiciousTradesInsertingUnitOfWork(_dbContextOptions, _logger);
    }
}
