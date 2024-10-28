using WatchDogs.Contracts;

namespace WatchDogs.Application;

public class WatcherBackgroundService : BackgroundService
{
    //private readonly IServiceScopeFactory _scopeFactory;
    private readonly IWatcher _watcher;

    public WatcherBackgroundService(/*IServiceScopeFactory scopeFactory,*/ IWatcher watcher)
    {
        //_scopeFactory = scopeFactory;
        _watcher = watcher;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        //using var scope = _scopeFactory.CreateScope();

        //var watcher = scope.ServiceProvider.GetRequiredService<IWatcher>();
        //var watcher = scope.ServiceProvider.GetRequiredService<IWatcher>();

        await _watcher.StartAsync(stoppingToken);
    }

}
