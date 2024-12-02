namespace WatchDogs.Contracts;
public interface ITradeInserter
{
    Task InsertTradeDatatoDbAsync(IEnumerable<Trade> data); 
} 