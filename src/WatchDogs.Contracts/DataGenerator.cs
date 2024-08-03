using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bogus;

namespace WatchDogs.Contracts;

//**Deal #2**, Balance 10 000, Sell GBPUSD 0.2 lots at 2019-05-12 14:43:23 

public class DataGenerator
{
    // This holds an information or set of rules for generating fake data for TradeModel

    Faker<TradeModel> tradeModelFake;

    public DataGenerator()
    {
        // This field is optional and it allows us to replicate the results (everytime we runs an application, the app will generate same results)
        Randomizer.Seed = new Random(123);

        tradeModelFake = new Faker<TradeModel>()
            .RuleFor(u => u.DealsGuid, f => f.Finance.Random.Guid())
            .RuleFor(u => u.Currency, f => f.Finance.Currency().Code)
            .RuleFor(u => u.TimeStamp, f => f.DateTimeReference.Value)
            .RuleFor(u => u.Action, f => f.PickRandom<TradeAction>())
            .RuleFor(u => u.Lot, f => f.Random.Decimal())
            .RuleFor(u => u.AcountBalance, f => f.Finance.Random.Decimal(/*amount yet to be inserted*/)
            );
    }

}
