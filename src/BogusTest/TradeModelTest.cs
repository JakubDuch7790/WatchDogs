using Bogus.DataSets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BogusTest;
public record TradeModel

//**Deal #2**, Balance 10 000, Sell GBPUSD 0.2 lots at 2019-05-12 14:43:23 


//Guid DealsGuid,
//decimal AcountBalance,
//TradeAction Action,
//DateTimeOffset TimeStamp,
//string Currency,
//decimal Lot

//Why Tim want to make this record instead of class??? Ask Mato.

{
    public Guid DealsGuid { get; init; }
    public decimal AccountBalance { get; init; }
    public TradeAction Action { get; init; }
    public DateTimeOffset TimeStamp { get; init; }
    public string Currency { get; init; }
    public decimal Lot { get; init; }


    public override string ToString()
    {
        return $"**Deal: {DealsGuid}, Balance {AccountBalance}, {Action} {Currency} {Lot} at {TimeStamp}\n";
    }

}

public enum TradeAction
{
    Buy,
    Sell
}
