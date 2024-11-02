using Serilog;
using WatchDogs.Contracts;

namespace WatchDogs.Persistence.EntityFramework;
public class SuspiciousTradesInserter : ISuspiciousDealInserter
{
    private readonly SuspiciousTradesDbContext _context;
    private readonly ILogger _logger;

    public SuspiciousTradesInserter(SuspiciousTradesDbContext suspiciousTradesDbContext, ILogger logger)
    {
        _context = suspiciousTradesDbContext;
        _logger = logger;
    }

    public async Task InsertAsync(IEnumerable<Trade> data)
    {
        try
        {
            await _context.SuspiciousTrades.AddRangeAsync(data);

        }
        catch (Exception ex)
        {
            _logger.Error($"{ex.Message}");
        }
    }

    public async Task SaveAsync()
    {
        await _context.SaveChangesAsync();
    }
}
