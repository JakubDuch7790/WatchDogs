using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bogus;

namespace WatchDogs.Contracts;

public class DataGenerator
{
    // This holds an information or set of rules for generating fake data for TradeModel

    Faker<TradeModel> tradeModelFake;

    public DataGenerator()
    {
        // This field is optional and it allows us to replicate the results (everytime we runs an application, the app will generate same results)
        Randomizer.Seed = new Random(123);

        tradeModelFake = new Faker<TradeModel>()
            .RuleFor(u => u.Currency, f => f.Finance.Currency().Code);
    }

}
