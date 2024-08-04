using Bogus;
using Bogus.DataSets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BogusTest;

//**Deal #2**, Balance 10 000, Sell GBPUSD 0.2 lots at 2019-05-12 14:43:23 

public class DataGeneratorTest
{
    // This holds an information or set of rules for generating fake data for TradeModel

    Faker<TradeModel> tradeModelFake;

    public DataGeneratorTest()
    {
        // This field is optional and it allows us to replicate the results (everytime we runs an application, the app will generate same results)
        Randomizer.Seed = new Random(123);

        tradeModelFake = new Faker<TradeModel>()
            .RuleFor(u => u.DealsGuid, f => f.Finance.Random.Guid())
            .RuleFor(u => u.Currency, f => GetRandomCurrencyPair(f))
            .RuleFor(u => u.TimeStamp, f => f.Date.RecentOffset(0))
            .RuleFor(u => u.Action, f => f.PickRandom<TradeAction>())
            .RuleFor(u => u.Lot, f => f.Random.Decimal())
            .RuleFor(u => u.AccountBalance, f => f.Finance.Random.Decimal(/*amount yet to be inserted*/)
            );
    }

    

    public TradeModel GenerateFakeTrade()
    {
        return tradeModelFake.Generate();
    }

    public IEnumerable<TradeModel> GenerateFakeTrades()
    {
        return tradeModelFake.GenerateForever();
    }

    public void LoadFakeData()
    {
        var fakeTrades = GenerateFakeTrades().Take(10);

        foreach (var trade in fakeTrades)
        {
            Console.WriteLine(trade);
        }
    }

    private string GetRandomCurrencyPair(Faker f)
    {
        var currency1 = f.Finance.Currency().Code;
        var currency2 = f.Finance.Currency().Code;

        // loop below ensure the two currencies are different

        while (currency1 == currency2)
        {
            currency2 = f.Finance.Currency().Code;
        }

        return currency1 + currency2;
    }

}
