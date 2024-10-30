namespace WatchDogs.Contracts;
public interface IDataInserter
{
    Task InsertTradeDatatoDbAsync(IEnumerable<Trade> data);
}
