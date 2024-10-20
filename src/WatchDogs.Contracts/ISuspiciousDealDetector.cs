using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WatchDogs.Contracts;
public interface ISuspiciousDealDetector
{
    Task<List<Trade>> LoadDealsAsync();
    Task SortTradesByCurrencyPairsAsync(List<Trade> trades);
    Task<List<Trade>> DetectSuspiciousDealsAsync(List<Trade> trades);
}
