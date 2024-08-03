using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bogus;

namespace WatchDogs.Contracts;

public record TradeModel(

    Guid DealsGuid,
    decimal AcountBalance,
    TradeAction Action,
    DateTimeOffset TimeStamp,
    string Currency,
    decimal Lot
    );
