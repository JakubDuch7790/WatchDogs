using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WatchDogs.Contracts;

namespace WatchDogs.Domain;

public class SuspiciousDealDetector : ISuspiciousDealDetector
{
    private readonly IDataLoader _dataLoader;
    private readonly SuspiciousDealsDetectorOptions _options;
    //private List<ICurrencyBucket> _currencyBuckets = new List<ICurrencyBucket>();
    public ConcurrentDictionary<string, ICurrencyBucket> _currencyTradesPairs = new ConcurrentDictionary<string, ICurrencyBucket>();
    public SuspiciousDealDetector(IDataLoader dataLoader, SuspiciousDealsDetectorOptions options)
    {
        _dataLoader = dataLoader;
        _options = options;
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
        return (trade.Lot * _options.NanoLot) / trade.AccountBalance;
    }

    //private async Task SortSingleCurrencyPair()

}
