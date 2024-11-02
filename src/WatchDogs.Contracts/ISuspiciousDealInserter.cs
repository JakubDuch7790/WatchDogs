namespace WatchDogs.Contracts;
public interface ISuspiciousDealInserter : ITradeInserter
{
    Task SaveAsync();
}
