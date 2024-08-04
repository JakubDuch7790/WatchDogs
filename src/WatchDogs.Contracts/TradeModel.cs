using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bogus;

namespace WatchDogs.Contracts;

public record TradeModel

    //Guid DealsGuid,
    //decimal AcountBalance,
    //TradeAction Action,
    //DateTimeOffset TimeStamp,
    //string Currency,
    //decimal Lot

    //Why Tim want to make this record instead of class??? Ask Mato.

{
    public Guid DealsGuid { get; set; }
    public decimal AccountBalance { get; set; }
    public TradeAction Action { get; set; }
    public DateTimeOffset TimeStamp { get; set; }
    public string Currency { get; set; }
    public decimal Lot { get; set; }
}