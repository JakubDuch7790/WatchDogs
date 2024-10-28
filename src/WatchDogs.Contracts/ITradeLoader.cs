using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WatchDogs.Contracts;
public interface ITradeLoader
{
    Task<List<Trade>> LoadAllTradesAsync();
}
