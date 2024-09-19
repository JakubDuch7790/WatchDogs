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

        var shmulRosenzweig = GroupTradesByTimestamp(trades);
        var wulfsschanze = GroupTradesByTimestamps(trades, TimeSpan.FromSeconds(1));
        //var reichskanzlei;

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
    // It seems that YES WE CAN. Yet we have to find a place for it's usage.

    // Let's try order trades by Timestamp first then we add 1 sec tolerance

    private List<List<Trade>> GroupTradesByTimestamp(List<Trade> trades)
    {
        return trades.GroupBy(trade => trade.TimeStamp)
            .Select(group => group.ToList())
            .ToList();
    }

    // Now we add this 1 sec tolerance because now it's shit.
    private List<List<Trade>> GroupTradesByTimestampWithTimeTolerance(List<Trade> trades, TimeSpan timeToleranceInSeconds)
    {
        return trades.GroupBy(trade => trade.TimeStamp)
            .Select(group => group.ToList())
            .ToList();
    }

    private IEnumerable<List<Trade>> GroupTradesByTimestamps(List<Trade> trades, TimeSpan tolerance)
    {
        // Sort trades by timestamp first
        trades = trades.OrderBy(trade => trade.TimeStamp).ToList();

        var groupedTrades = new List<List<Trade>>();
        var currentGroup = new List<Trade>();

        foreach (var trade in trades)
        {
            if (currentGroup.Count == 0)
            {
                currentGroup.Add(trade);
            }
            else
            {
                var lastTrade = currentGroup.Last();
                var timeDifference = trade.TimeStamp - lastTrade.TimeStamp;

                // Let's hope this shit works
                if (timeDifference.Duration() <= tolerance)
                {
                    currentGroup.Add(trade);
                }
                else
                {
                    groupedTrades.Add(currentGroup);
                    currentGroup = new List<Trade> { trade };
                }
            }
        }

        // Ending
        if (currentGroup.Count > 0)
        {
            groupedTrades.Add(currentGroup);
        }
        // Hmmmm
        return groupedTrades.Where(group => group.Count > 1);
    }
}
