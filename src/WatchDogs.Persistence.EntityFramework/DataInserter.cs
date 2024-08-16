using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WatchDogs.Contracts;

namespace WatchDogs.Persistence.EntityFramework;
public class DataInserter
{
    private readonly ApplicationDbContext _context;

    public DataInserter(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task InsertTradeDatatoDbAsync(IEnumerable<Trade> data)
    {
        foreach (var trade in data)
        {
            await _context.Trades.AddAsync(trade);
        }

        await _context.SaveChangesAsync();
    }
}
