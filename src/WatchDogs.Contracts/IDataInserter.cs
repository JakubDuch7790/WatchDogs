using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WatchDogs.Contracts;
public interface IDataInserter
{
    Task InsertTradeDatatoDbAsync(IEnumerable<Trade> data); 
    Task InsertTradeDatatoDbAsyncccc(IEnumerable<Trade> data);
}