
namespace WatchDogs.Infrastructure.FakeSource;

public interface IWatcher
{
    Task StartAsync(CancellationToken token = default);
}