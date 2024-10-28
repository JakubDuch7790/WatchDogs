using WatchDogs.Contracts;

namespace WatchDogs.Persistence.EntityFramework;

public class TradeInserter : ITradeInserter
{
    private readonly ApplicationDbContext _context;

    public TradeInserter(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task InsertTradeDatatoDbAsync(IEnumerable<Trade> data)
    {
        try
        {
            await _context.Trades.AddRangeAsync(data);

            //await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            //TODO: add logging
        }
    }

    private async Task AddSingleTrade(Trade trade)
    {
        await _context.Trades.AddAsync(trade);
    }
}
