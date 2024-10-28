using Microsoft.EntityFrameworkCore;
using WatchDogs.Contracts;

namespace WatchDogs.Persistence.EntityFramework;
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                    
public class UnitOfWorkFactory : IUnitOfWorkFactory
{
    private readonly DbContextOptions<ApplicationDbContext> _dbContextOptions;

    public UnitOfWorkFactory(DbContextOptions<ApplicationDbContext> dbContextOptions)
    {
        _dbContextOptions = dbContextOptions;
    }

    public IUnitOfWork Create()
    {
        return new EntityFrameworkUnitOfWork(_dbContextOptions);
    }
}
