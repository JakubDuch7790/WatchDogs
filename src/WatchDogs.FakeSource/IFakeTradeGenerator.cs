
using WatchDogs.Contracts;

namespace WatchDogs.FakeSource;

public interface IFakeTradeGenerator
{
    IEnumerable<Trade> LoadFakeData();
}