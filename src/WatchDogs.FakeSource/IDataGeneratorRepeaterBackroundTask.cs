
namespace WatchDogs.FakeSource;

public interface IDataGeneratorRepeaterBackroundTask
{
    Task StartAsync(CancellationToken token = default);
    Task StopAsync();
}