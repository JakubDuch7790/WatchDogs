namespace WatchDogs.Contracts;
public interface ITradeLoader
{
    Task<List<Trade>> LoadAllTradesAsync();
}
