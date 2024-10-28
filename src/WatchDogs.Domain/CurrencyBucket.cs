using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WatchDogs.Contracts;

namespace WatchDogs.Domain;
public class CurrencyBucket
{
    public string CurrencyPair { get; set; }
    public List<Trade> Trades { get; set; } = new List<Trade>();

    public CurrencyBucket(string currencyPair)
    {
        CurrencyPair = currencyPair;
    }
}
