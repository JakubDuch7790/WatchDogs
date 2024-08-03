using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WatchDogs.Contracts;

/* #### Examples
**Example 1**  
List of incoming deals:  
**Deal #1**, Balance 10 000, Buy EURUSD 1 lot at 2019-05-12 14:43:12  
**Deal #2**, Balance 10 000, Sell GBPUSD 0.2 lots at 2019-05-12 14:43:23  
**Deal #3**, Balance 10 000, Sell GBPUSD 0.21 lot at 2019-05-12 14:43:24 _<- triggered match with deal #2_  

**Example 2**  
List of incoming deals:  
**Deal #1**, Balance 10 000, Buy EURUSD 1 lot at 2019-05-12 14:43:12  
**Deal #2**, Balance 10 000, Sell GBPUSD 0.2 lots at 2019-05-12 14:43:23  
**Deal #3**, Balance 1 000, Sell GBPUSD 1.2 lots at 2019-05-12 14:43:23  
**Deal #4**, Balance 10 000, Sell GBPUSD 0.21 lot at 2019-05-12 14:43:24 _<- triggered match with deal #2_  
**Deal #5**, Balance 20 000, Sell GBPUSD 0.4 lot at 2019-05-12 14:43:24 _<- triggered match with deal #2 and deal #4_   */

public class Trade : ITrade
{
    public int DealID { get; set; }
    public int Balance { get; set; }
    public string Action { get; set; }
    public string Currency { get; set; }
    public decimal Lot { get; set; }
    public DateTime TimeStamp { get; set; }

    public Trade(int dealID, int balance, string action, string currency, decimal lot, DateTime timeStamp)
    {
        DealID = dealID;
        Balance = balance;
        Action = action;
        Currency = currency;
        Lot = lot;
        TimeStamp = timeStamp;
    }

}
