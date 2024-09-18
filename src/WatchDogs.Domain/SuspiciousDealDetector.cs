using Microsoft.Extensions.Options;
using System.Collections.Concurrent;
using WatchDogs.Contracts;

namespace WatchDogs.Domain;

public class SuspiciousDealDetector : ISuspiciousDealDetector
{
    private const decimal VolumeToBalanceTolerance = 0.05M;
    private readonly IDataLoader _dataLoader;
    private readonly SuspiciousDealDetectorOptions _suspiciousDealDetectorOptions;

    //private List<ICurrencyBucket> _currencyBuckets = new List<ICurrencyBucket>();
    public ConcurrentDictionary<string, ICurrencyBucket> _currencyTradesPairs = new ConcurrentDictionary<string, ICurrencyBucket>();
    public SuspiciousDealDetector(IDataLoader dataLoader, IOptions<SuspiciousDealDetectorOptions> options)
    {
        _dataLoader = dataLoader;
        _suspiciousDealDetectorOptions = options.Value;
    }

    public async Task<List<Trade>> LoadDealsAsync()
    {
        return await _dataLoader.LoadAllTradesAsync();
    }
    public async Task SortTradesByCurrencyPairsAsync(List<Trade> trades)
    {
        //var trades = await LoadDealsAsync();

        foreach (var trade in trades)
        {
            var HeilHilter = VolumeToBalanceRatioCalculator(trade);

            if(!_currencyTradesPairs.ContainsKey(trade.Currency))
            {
                _currencyTradesPairs.TryAdd(trade.Currency, new CurrencyBucket(trade.Currency));
                _currencyTradesPairs[trade.Currency].Trades.Add(trade);

                //trade.IsProccessed = true;
            }
            else
            {
                _currencyTradesPairs[trade.Currency].Trades.Add(trade);

                //trade.IsProccessed = true;

            }
        }
        var ss = DetectSuspiciousDeals();
    }
    public async Task<List<Trade>> DetectSuspiciousDeals()
    {
        var SS = new List<Trade>();



        foreach(var currencyPair in _currencyTradesPairs.Values)
        {
            for (int i = 0; i < currencyPair.Trades.Count; i++)
            {
                if (currencyPair.Trades[i].TimeStamp == currencyPair.Trades[i + 1].TimeStamp)
                {

                }
            }

                var temp = currencyPair.Trades.OrderBy(trade => trade.TimeStamp).ToList();/*.Where(group => group.Count() > 1).ToList();*/
            //SS.Add(temp);
        }
        return SS;
    }

    // Let's first find out if we can calculate that Volume-to-Balance ratio thing
    private decimal VolumeToBalanceRatioCalculator(Trade trade)
    {
        return (trade.Lot * _suspiciousDealDetectorOptions.MicroLot) / trade.AccountBalance;
    }
    private bool HasAcceptableVolumeToBalanceDifference(Trade trade1, Trade trade2)
    {
        var VolumeToBalanceRatio1 = VolumeToBalanceRatioCalculator(trade1);
        var VolumeToBalanceRatio2 = VolumeToBalanceRatioCalculator(trade2);

        var ratioDifference = Math.Abs(VolumeToBalanceRatio1 - VolumeToBalanceRatio2);

        return ratioDifference < VolumeToBalanceTolerance;
    }
}
