
namespace WatchDogs.FakeSource;

public interface IWatcher
{
    Task StartAsync(CancellationToken token = default);
}