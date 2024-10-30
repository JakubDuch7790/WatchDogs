namespace WatchDogs.Contracts;
public interface ISuspiciousDealDetector
{
    Task<List<List<Trade>>> DetectSuspiciousDealsAsync(List<Trade> trades);

    Task<List<Trade>> LoadDealsAsync();

    Task StoreSuspiciousTradesAsync(List<List<Trade>> trades);

}
