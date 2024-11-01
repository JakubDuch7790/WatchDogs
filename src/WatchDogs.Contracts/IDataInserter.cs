namespace WatchDogs.Contracts;
public interface IDataInserter
{
    Task InsertAsync(IEnumerable<Trade> data);
}
