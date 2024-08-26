using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WatchDogs.Contracts;
public interface ICurrencyBucket
{
    string CurrencyPair {  get; set; }
    Trade Trade { get; set; }
    List<Trade> Trades { get; set; }
}
