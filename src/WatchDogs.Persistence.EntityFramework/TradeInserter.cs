using Serilog;
using WatchDogs.Contracts;

namespace WatchDogs.Persistence.EntityFramework;

public class TradeInserter : ITradeInserter
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger _logger;

    public TradeInserter(ApplicationDbContext context, ILogger logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task InsertTradeDatatoDbAsync(IEnumerable<Trade> data)
    {
        try
        {
            await _context.Trades.AddRangeAsync(data);

        }
        catch (Exception ex)
        {
            _logger.Error($"{ex.Message}");
        }
    }
}
