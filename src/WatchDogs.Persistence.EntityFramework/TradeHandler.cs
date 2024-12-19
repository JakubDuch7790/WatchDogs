using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WatchDogs.Contracts;

namespace WatchDogs.Persistence.EntityFramework;
public class TradeHandler : ITradeHandler
{
    private readonly ApplicationDbContext _context;

    public TradeHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task HandleTradeAsync(Trade trade)
    {
        _context.Remove<Trade>(trade);
        await _context.SaveChangesAsync();
    }


}
