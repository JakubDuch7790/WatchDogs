namespace WatchDogs.Contracts;
public interface IWatcher
{
    Task StartAsync(CancellationToken token = default);
}