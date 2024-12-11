using Microsoft.EntityFrameworkCore;
using Serilog;
using WatchDogs.Contracts;

namespace WatchDogs.Persistence.EntityFramework;
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                    
public class UnitOfWorkFactory : IUnitOfWorkFactory
{
    private readonly DbContextOptions<ApplicationDbContext> _dbContextOptions;
    private readonly ILogger _logger;

    public UnitOfWorkFactory(DbContextOptions<ApplicationDbContext> dbContextOptions, ILogger logger)
    {
        _dbContextOptions = dbContextOptions;
        _logger = logger;
    }

    public IUnitOfWork Create()
    {
        return new EntityFrameworkUnitOfWork(_dbContextOptions, _logger);
    }
}
