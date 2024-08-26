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
    private List<ICurrencyBucket> _currencyBuckets = new List<ICurrencyBucket>();
    private ConcurrentDictionary<string, ICurrencyBucket> _currencyTradesPairs;
    public SuspiciousDealDetector(IDataLoader dataLoader)
    {
        _dataLoader = dataLoader;
    }

    public async Task<List<Trade>> LoadDealsAsync()
    {
        return await _dataLoader.LoadAllTradesAsync();
    }
    public async Task SortTradesByCurrencyPairsAsync()
    {
        var trades = await LoadDealsAsync();

        foreach (var trade in trades)
        {
            if(!_currencyTradesPairs.ContainsKey(trade.Currency))
            {
                _currencyTradesPairs.TryAdd(trade.Currency, new CurrencyBucket(trade.Currency));
                _currencyTradesPairs[trade.Currency].Trades.Add(trade);
            }
            else
            {
                _currencyTradesPairs[trade.Currency].Trades.Add(trade);
            }
        }
    }

    //private async Task SortSingleCurrencyPair()

}
