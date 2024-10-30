namespace WatchDogs.Contracts;
public interface ISuspiciousDealInserter : IDataInserter
{
    Task SaveAsync();
}
