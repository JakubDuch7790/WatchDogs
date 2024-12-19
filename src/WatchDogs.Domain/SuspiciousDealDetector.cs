using Microsoft.Extensions.Options;
using System.Collections.Concurrent;
using System.Diagnostics;
using WatchDogs.Contracts;
using ILogger = Serilog.ILogger;

namespace WatchDogs.Domain;

public class SuspiciousDealDetector : ISuspiciousDealDetector
{
    private const decimal VolumeToBalanceTolerance = 0.05M;
    private static readonly TimeSpan TimeDifferTolerance = TimeSpan.FromSeconds(1);
    private readonly ITradeLoader _dataLoader;
    private readonly SuspiciousDealDetectorOptions _suspiciousDealDetectorOptions;
    private readonly ILogger _logger;
    private readonly ISuspiciousDealInserter _suspiciousDealInserter;

    private readonly ITradeHandler _tradeHandler;

    public ConcurrentDictionary<string, CurrencyBucket> _currencyTradesPairs = new ConcurrentDictionary<string, CurrencyBucket>();
    public SuspiciousDealDetector(ITradeLoader dataLoader, IOptions<SuspiciousDealDetectorOptions> options, ILogger logger, ISuspiciousDealInserter suspiciousDealInserter, ITradeHandler tradeHandler)
    {
        _dataLoader = dataLoader;
        _suspiciousDealDetectorOptions = options.Value;
        _logger = logger;
        _suspiciousDealInserter = suspiciousDealInserter;
        _tradeHandler = tradeHandler;
    }

    public async Task<List<Trade>> LoadDealsAsync()
    {
        return await _dataLoader.LoadAllTradesAsync();
    }

    #region One-to-many principle
    public async Task<Trade> LoadOneDealAtTimeAsync()
    {
        return await _dataLoader.LoadOneTradeAtTimeAsync();
    }

    public async Task RemoveCurrentTradeAndMoveNextAsync(Trade trade)
    {
        await _tradeHandler.HandleTradeAsync(trade);
    }

    public async Task DetectAsync(Trade incomingTrade)
    {
        var bucket = CurrencyBucketSort(incomingTrade);

        var suspiciousTrades = CompareTradeWithBucket(incomingTrade, bucket.Trades);

        SuspiciousDealsLogVisulizer(suspiciousTrades);
    }

    public CurrencyBucket CurrencyBucketSort(Trade incomingTrade)
    {
        var bucket = _currencyTradesPairs.GetOrAdd(incomingTrade.Currency, x => new CurrencyBucket(incomingTrade.Currency));
        bucket.Trades.Add(incomingTrade);

        return bucket;
    }
    private List<Trade> CompareTradeWithBucket(Trade incomingTrade, List<Trade> bucketTrades)
    {
        var suspiciousTrades = new List<Trade>();

        foreach (var trade in bucketTrades)
        {
            if (trade == incomingTrade) continue;

            if (IsWithinTimeTolerance(incomingTrade, trade) &&
                IsSameAction(incomingTrade, trade) &&
                HasSuspiciousVolumeToBalanceDifference(incomingTrade, trade))
            {
                suspiciousTrades.Add(trade);
            }
        }

        return suspiciousTrades;
    }

    private bool IsWithinTimeTolerance(Trade incomingTrade, Trade tradeFromBucket)
    {
        return Math.Abs((incomingTrade.TimeStamp - tradeFromBucket.TimeStamp).TotalSeconds) <= TimeDifferTolerance.TotalSeconds;
    }

    private bool IsSameAction(Trade incomingTrade, Trade tradeFromBucket)
    {
        return incomingTrade.Action == tradeFromBucket.Action;
    }

    private bool HasSuspiciousVolumeToBalanceDifference(Trade incomingTrade, Trade tradeFromBucket)
    {
        var VolumeToBalanceRatio1 = VolumeToBalanceRatioCalculator(incomingTrade);
        var VolumeToBalanceRatio2 = VolumeToBalanceRatioCalculator(tradeFromBucket);

        var ratioDifference = Math.Abs(VolumeToBalanceRatio1 - VolumeToBalanceRatio2);

        return Math.Abs(VolumeToBalanceRatio1 - VolumeToBalanceRatio2) < VolumeToBalanceTolerance;
    }
    private void SuspiciousDealsLogVisulizer(List<Trade> tradesInLists)
    {
        foreach (var trade in tradesInLists)
        {
            string tradesDetails = string.Join(", ", tradesInLists.Select(trade => trade.ToString()));

            _logger.Information($"Suspicious trades detected:\n {tradesDetails}");
        }
    }


    #endregion

    public async Task<List<List<Trade>>> DetectSuspiciousDealsAsync(List<Trade> trades)
    {
        List<List<Trade>> dealsAlreadyFilteredByCurrencyPairAndTimeStamp = new();

        await SortTradesByCurrencyPairsAsync(trades);

        var dealsInBuckets = _currencyTradesPairs.Values;

        _logger.Information($"Filtering trades into currency  . \n" +
            $"{dealsInBuckets.Count} buckets found.");

        foreach (var bucket in dealsInBuckets)
        {
            var tradesFromOneBucket = bucket.Trades;

            var filteredGroups = GroupTradesByTimestampWithTimeTolerance(tradesFromOneBucket, SuspiciousDealDetector.TimeDifferTolerance);
              
            dealsAlreadyFilteredByCurrencyPairAndTimeStamp.AddRange(filteredGroups);
        }

        _logger.Information($"Filtering by Timestamp. " +
            $"{dealsAlreadyFilteredByCurrencyPairAndTimeStamp.Count} buckets remains. {TradesTotalNumber(dealsAlreadyFilteredByCurrencyPairAndTimeStamp)} trades total. \n");

        if(dealsAlreadyFilteredByCurrencyPairAndTimeStamp.Count > 0)
        {
            var dealsAlreadyFilteredByCurrencyPairAndTimeStampAndAction = TradeActionFilter(dealsAlreadyFilteredByCurrencyPairAndTimeStamp);

            _logger.Information($"Filtering by Action. {TradesTotalNumber(dealsAlreadyFilteredByCurrencyPairAndTimeStampAndAction)} trades total");

            foreach(var deals in dealsAlreadyFilteredByCurrencyPairAndTimeStampAndAction)
            {
                var finalFilter = differenceInVolumeToBalanceRatioFilter(deals);
            }

            _logger.Information($"Final filtering by Volume to balance ratio. {TradesTotalNumber(dealsAlreadyFilteredByCurrencyPairAndTimeStampAndAction)} trades total");

            dealsAlreadyFilteredByCurrencyPairAndTimeStampAndAction.RemoveAll(list => !list.Any());

            SuspiciousDealsLogVisulizer(dealsAlreadyFilteredByCurrencyPairAndTimeStampAndAction);

            return dealsAlreadyFilteredByCurrencyPairAndTimeStampAndAction;

        }

        return dealsAlreadyFilteredByCurrencyPairAndTimeStamp;
    }

    #region Currency Bucket Sort

    public async Task SortTradesByCurrencyPairsAsync(List<Trade> trades)
    {

        foreach (var trade in trades)
        {
            if (!_currencyTradesPairs.ContainsKey(trade.Currency))
            {
                _currencyTradesPairs.AddOrUpdate(trade.Currency, new CurrencyBucket(trade.Currency), (currency, bucket) =>
                {
                    bucket.Trades.Add(trade);

                    return bucket;
                });
            }

            else
            {
                _currencyTradesPairs[trade.Currency].Trades.Add(trade);
            }
        }
    }

    #endregion

    #region Timestamp filter

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
    }

    #endregion

    #region Buy/Sell Filter

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

    #endregion

    #region Volume To Balance Ratio Filter
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

        if (trades.Count == 1)
        {
            trades.Clear();
        }
        return trades;
    }

    #endregion

    #region Helper Methods
    private int TradesTotalNumber(IEnumerable<List<Trade>> trades)
    {
        int tradesTotal = 0;

        foreach (var list in trades)
        {
            foreach (var trade in list)
            {
                tradesTotal++;
            }
        }
        return tradesTotal;
    }
    private void SuspiciousDealsLogVisulizer(List<List<Trade>> tradesInLists)
    {

        foreach (var list in tradesInLists)
        {
            string tradesDetails = string.Join(", ", list.Select(trade => trade.ToString()));

            _logger.Information($"Suspicious trades detected:\n {tradesDetails}");
        }
    }

    #endregion

    public async Task StoreSuspiciousTradesAsync(List<List<Trade>> trades)
    {
        List<Trade> tradesToInsert = new List<Trade>();

        foreach (var listOfTrades in trades)
        {
            foreach (var trade in listOfTrades)
            {
                tradesToInsert.Add(trade);
            }
        }

        await _suspiciousDealInserter.InsertAsync(tradesToInsert);
        await _suspiciousDealInserter.SaveAsync();
    }

}
