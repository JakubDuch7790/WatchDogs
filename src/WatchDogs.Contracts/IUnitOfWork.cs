namespace WatchDogs.Contracts;

public interface IUnitOfWork : IDisposable
{
    ITradeInserter DataInserter { get; }

    Task SaveAsync();
}
