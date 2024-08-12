
namespace WatchDogs.FakeSource;

public interface IDataGeneratorRepeaterBackroundTask
{
    void Start();
    Task StopAsync();
}