using WatchDogs.Contracts;

namespace WatchDogs.Persistence.EntityFramework;
public interface IDataInserter
{
    Task InsertTradeDatatoDbAsync(IEnumerable<Trade> data);
}