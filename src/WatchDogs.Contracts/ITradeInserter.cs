namespace WatchDogs.Contracts;
public interface ITradeInserter
{
    Task InsertAsync(IEnumerable<Trade> data); 
} 