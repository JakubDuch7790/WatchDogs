using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bogus;

namespace WatchDogs.Contracts;

public record Trade

    //Guid Id,
    //decimal AcountBalance,
    //TradeAction Action,
    //DateTimeOffset TimeStamp,
    //string Currency,
    //decimal Lot

{
    public Guid Id { get; init; }
    public decimal AccountBalance { get; init; }
    public TradeAction Action { get; init; }
    public DateTimeOffset TimeStamp { get; init; }
    public string Currency { get; init; }
    public decimal Lot { get; init; }

    public override string ToString()
    {
        return $"**Deal: {Id}, Balance {AccountBalance}, {Action} {Currency} {Lot} at {TimeStamp}\n";
    }

}