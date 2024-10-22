namespace WatchDogs.Contracts;

public interface IUnitOfWork : IDisposable
{
    IDataInserter DataInserter { get; }

    Task SaveAsync();
}
