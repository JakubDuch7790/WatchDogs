namespace WatchDogs.Contracts;

public interface IUnitOfWork : IDisposable
{
    ITradeInserter TradeInserter { get; }

    Task SaveAsync();
}
