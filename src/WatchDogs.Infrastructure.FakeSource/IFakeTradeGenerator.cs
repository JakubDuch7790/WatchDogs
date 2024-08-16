
using WatchDogs.Contracts;

namespace WatchDogs.Infrastructure.FakeSource;

public interface IFakeTradeGenerator
{
    IEnumerable<Trade> LoadFakeData();
}