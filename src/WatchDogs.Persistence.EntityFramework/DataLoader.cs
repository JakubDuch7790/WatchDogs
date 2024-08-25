using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WatchDogs.Contracts;

namespace WatchDogs.Persistence.EntityFramework;

public class DataLoader : IDataLoader
{
    private readonly ApplicationDbContext _context;

    public DataLoader(ApplicationDbContext context)
    {
        _context = context;
    }

    public void LoadAllTradesAsync()
    {
       _context.Trades.ToList();
    }
}
