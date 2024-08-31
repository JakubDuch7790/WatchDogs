﻿using Microsoft.EntityFrameworkCore;
using WatchDogs.Contracts;

namespace WatchDogs.Persistence.EntityFramework;

public class EntityFrameworkUnitOfWorkFactory //TODO: create an interface
{
    private readonly DbContextOptions<ApplicationDbContext> _dbContextOptions;

    public EntityFrameworkUnitOfWorkFactory(DbContextOptions<ApplicationDbContext> dbContextOptions)
    {
        _dbContextOptions = dbContextOptions;
    }

    public IUnitOfWork Create()
    {
        return new EntityFrameworkUnitOfWork(_dbContextOptions);
    }
}
