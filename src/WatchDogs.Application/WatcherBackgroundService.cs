using WatchDogs.Contracts;

namespace WatchDogs.Application;

public class WatcherBackgroundService : BackgroundService
{
    private readonly IWatcher _watcher;

    public WatcherBackgroundService(IWatcher watcher)
    {
        _watcher = watcher;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _watcher.StartAsync(stoppingToken);
    }
}
