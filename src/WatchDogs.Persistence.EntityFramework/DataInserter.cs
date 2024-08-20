using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WatchDogs.Contracts;

namespace WatchDogs.Persistence.EntityFramework;
public class DataInserter : IDataInserter
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
    public async Task InsertTradeDatatoDbAsyncccc(IEnumerable<Trade> data)
    {
        // Toto neslo ale List<Task> ide
        List<Task<IEnumerable<Trade>>> tasks = new();

        List<Task> taskss = new();

        foreach (var trade in data)
        {
            taskss.Add(AddSingleTrade(trade));
        }

        await Task.WhenAll(taskss);

        await _context.SaveChangesAsync();
    }

    private async Task AddSingleTrade(Trade trade)
    {
        await _context.Trades.AddAsync(trade);
        //await _context.SaveChangesAsync();
    }
}
