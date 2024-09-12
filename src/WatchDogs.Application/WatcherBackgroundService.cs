
using WatchDogs.Contracts;
using WatchDogs.Persistence.EntityFramework;

namespace WatchDogs.Application;

public class WatcherBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    //private readonly IEnumerable<IWatcher> _watchers;

    public WatcherBackgroundService(IServiceScopeFactory scopeFactory/*, IEnumerable<IWatcher> watchers*/)
    {
        _scopeFactory = scopeFactory;
        //_watchers = watchers;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var scope = _scopeFactory.CreateScope();

        var watcher = scope.ServiceProvider.GetRequiredService<IWatcher>();
        //foreach (var watcher in _watchers)
        //{
        //    // Ja len tak ich viem startnut asynchronne.
        //    await watcher.StartAsync(stoppingToken);
        //}
        await watcher.StartAsync(stoppingToken);

    }

}
