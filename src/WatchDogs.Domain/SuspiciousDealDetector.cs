using Microsoft.Extensions.Options;
using Serilog;
using System.Collections.Concurrent;
using WatchDogs.Contracts;
using ILogger = Serilog.ILogger;

namespace WatchDogs.Domain;

public class SuspiciousDealDetector : ISuspiciousDealDetector
{
    private const decimal VolumeToBalanceTolerance = 0.05M;
    private static readonly TimeSpan TimeDifferTolerance = TimeSpan.FromSeconds(1);
    private readonly IDataLoader _dataLoader;
    private readonly SuspiciousDealDetectorOptions _suspiciousDealDetectorOptions;
    private readonly ILogger _logger;

    public ConcurrentDictionary<string, ICurrencyBucket> _currencyTradesPairs = new ConcurrentDictionary<string, ICurrencyBucket>();
    public SuspiciousDealDetector(IDataLoader dataLoader, IOptions<SuspiciousDealDetectorOptions> options, ILogger logger)
    {
        _dataLoader = dataLoader;
        _suspiciousDealDetectorOptions = options.Value;
        _logger = logger;
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
    }
    
    public async Task<List<List<Trade>>> DetectSuspiciousDealsAsync(List<Trade> trades)
    {
        List<List<Trade>> dealsAlreadyFilteredByCurrencyPairAndTimeStamp = new();

        await SortTradesByCurrencyPairsAsync(trades);

        var dealsInBuckets = _currencyTradesPairs.Values;

        Log.Information($"Filtering trades into currency buckets. \n" +
            $"{dealsInBuckets.Count} buckets found.");

        foreach (var bucket in dealsInBuckets)
        {
            var tradesFromOneBucket = bucket.Trades;

            var filteredGroups = GroupTradesByTimestampWithTimeTolerance(tradesFromOneBucket, SuspiciousDealDetector.TimeDifferTolerance);

            dealsAlreadyFilteredByCurrencyPairAndTimeStamp.AddRange(filteredGroups);

        }

        Log.Information($"Filtering by Timestamp. " +
            $"{dealsAlreadyFilteredByCurrencyPairAndTimeStamp.Count} buckets remains. {TradesTotalNumber(dealsAlreadyFilteredByCurrencyPairAndTimeStamp)} trades total. \n");

        if(dealsAlreadyFilteredByCurrencyPairAndTimeStamp.Count > 0)
        {
            var dealsAlreadyFilteredByCurrencyPairAndTimeStampAndAction = TradeActionFilter(dealsAlreadyFilteredByCurrencyPairAndTimeStamp);

            Log.Information($"Filtering by Action. {TradesTotalNumber(dealsAlreadyFilteredByCurrencyPairAndTimeStampAndAction)} trades total");

            foreach(var deals in dealsAlreadyFilteredByCurrencyPairAndTimeStampAndAction)
            {
                var finalFilter = differenceInVolumeToBalanceRatioFilter(deals);
            }

            Log.Information($"Final filtering by Volume to balance ratio. {TradesTotalNumber(dealsAlreadyFilteredByCurrencyPairAndTimeStampAndAction)} trades total");

            dealsAlreadyFilteredByCurrencyPairAndTimeStampAndAction.RemoveAll(list => !list.Any());

            SuspiciousDealsLogVisulizer(dealsAlreadyFilteredByCurrencyPairAndTimeStampAndAction);

            return dealsAlreadyFilteredByCurrencyPairAndTimeStampAndAction;

        }


        return dealsAlreadyFilteredByCurrencyPairAndTimeStamp;
    }

    private void SuspiciousDealsLogVisulizer(List<List<Trade>> tradesInLists)
    {

        foreach(var list in tradesInLists)
        {
            string tradesDetails = string.Join(", ", list.Select(trade => trade.ToString()));

            Log.Information($"Suspicious trades detected:\n {tradesDetails}");
            //Log.Information($"Suspicious trades Detected {list.SelectMany(x => x.ToString())}");
        }
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

    // Filter for the difference in volume-to-balance ratio
    private List<Trade> differenceInVolumeToBalanceRatioFilter(List<Trade> trades)
    {
        List<Trade> tradesToRemove = new List<Trade>();

        for (int i = 0; i < trades.Count; i++)
        {
            for (int j = i + 1; j < trades.Count; j++)
            {
                var VolumeToBalanceRatio1 = VolumeToBalanceRatioCalculator(trades[i]);
                var VolumeToBalanceRatio2 = VolumeToBalanceRatioCalculator(trades[j]);

                var ratioDifference = Math.Abs(VolumeToBalanceRatio1 - VolumeToBalanceRatio2);

                if (ratioDifference >= VolumeToBalanceTolerance)
                {
                    tradesToRemove.Add(trades[i]);
                }
            }
        }

        foreach (var trade in tradesToRemove)
        {
            trades.Remove(trade);
        }

        if(trades.Count == 1)
        {
            trades.Clear();
        }
        return trades;
    }



    // It seems that YES WE CAN. Yet we have to find a place for it's usage.

    // Beware of IEnumerable
    private IEnumerable<List<Trade>> GroupTradesByTimestampWithTimeTolerance(List<Trade> trades, TimeSpan tolerance)
    {
        IEnumerable<List<Trade>> DealsAfterFilteringWithMoreThanOneDealInIt = new List<List<Trade>>();

        // 1. Step : Sorting trades by timestamp first to get chronological order
        trades = trades.OrderBy(trade => trade.TimeStamp).ToList();

        /* Variable groupedTrades contains List of Trades with more than one deal fulfilling criteria,
         * i.e. number of suspicious trades exceeds 1.
         currentGroup is self explanatory*/

        var groupedTrades = new List<List<Trade>>();
        var currentGroup = new List<Trade>();

        // 2. Step : Iteration over collection of trades

        foreach (var trade in trades)
        {
            if (currentGroup.Count == 0)
            {
                currentGroup.Add(trade);
            }
            else
            {
                // Setting last deal for comparison
                var lastTrade = currentGroup.Last();

                /* Checking time differ criteria
                 * In case of similiar time, add deal to the currentGroup for futher computing */

                var timeDifference = trade.TimeStamp - lastTrade.TimeStamp;

                // Comparing against tolerance
                if (timeDifference.Duration() <= tolerance)
                {
                    currentGroup.Add(trade);
                    _logger.Information("Trades with similiar time differ detected!");
                }
                else
                {
                    /* In this step the time differ vary so we are proceeding further and begin new iteration*/
                    groupedTrades.Add(currentGroup);
                    currentGroup = new List<Trade> { trade };
                }
            }
        }

        // 3. Step : Saving last currentGroup
        if (currentGroup.Count > 0)
        {
            groupedTrades.Add(currentGroup);
        }
        // 4. Step : Filtering for groups with more than one trade inside
        DealsAfterFilteringWithMoreThanOneDealInIt = groupedTrades.Where(group => group.Count > 1);
        return DealsAfterFilteringWithMoreThanOneDealInIt;

        // Conclusion : Functions seems to work fine. I will procceed with this implementation
    }

    //There are two versions provided because IDK which one will be implemented further
    private List<List<Trade>> TradeActionFilter(List<List<Trade>> trades)
    {
        var tradesToRemove = new List<List<Trade>>();

        foreach (var potentialSuspiciousTrades in trades)
        {
            var ActionToCheck = potentialSuspiciousTrades.First().Action;

            if (potentialSuspiciousTrades.Any(trade => trade.Action != ActionToCheck))
            {
                tradesToRemove.Add(potentialSuspiciousTrades);
            }
        }

        foreach (var trade in tradesToRemove)
        {
            trades.Remove(trade);
        }
        return trades;
    }
    private bool AreAllTradesOfSameActionVersion2(List<Trade> trades)
    {
        var ActionToCheck = trades.First().Action;

        if (trades.Any(trade => trade.Action != ActionToCheck))
        {
            return false;
        }

        return true;
    }

    private int TradesTotalNumber(IEnumerable<List<Trade>> trades)
    {
        int tradesTotal = 0;

        foreach(var list in trades)
        {
            foreach (var trade in list)
            {
                tradesTotal++;
            }
        }
        return tradesTotal;
    }

}
