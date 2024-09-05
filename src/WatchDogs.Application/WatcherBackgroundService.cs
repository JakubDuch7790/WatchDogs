
using WatchDogs.Contracts;
using WatchDogs.Persistence.EntityFramework;

namespace WatchDogs.Application;

public class WatcherBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;

    public WatcherBackgroundService(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _scopeFactory.CreateScope();

            var watcher = scope.ServiceProvider.GetRequiredService<IWatcher>();

            await watcher.StartAsync(stoppingToken);
        }
    }
}
