namespace WatchDogs.Contracts;
public interface ITradeLoader
{
    Task<List<Trade>> LoadAllTradesAsync();
    Task<Trade> LoadOneTradeAtTimeAsync();
}
