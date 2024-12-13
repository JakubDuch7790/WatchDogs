using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WatchDogs.Contracts;

public record Trade

    //Guid Id,
    //decimal AcountBalance,
    //TradeAction Action,
    //DateTimeOffset TimeStamp,
    //string CurrencyPair,
    //decimal Lot

{
    [Key]
    public Guid Id { get; init; }
    public decimal AccountBalance { get; init; }
    public TradeAction Action { get; init; }
    public DateTimeOffset TimeStamp { get; init; }
    public string CurrencyPair { get; init; }
    public decimal Lot { get; init; }

    public override string ToString()
    {
        return $"**Deal: {Id}, Balance {AccountBalance}, {Action} {CurrencyPair} {Lot} at {TimeStamp}\n";
    }

}