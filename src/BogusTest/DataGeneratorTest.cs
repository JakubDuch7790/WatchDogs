using Bogus;
using Bogus.DataSets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BogusTest;

//Deal #1, Balance 10 000, Buy EURUSD 1 lot at 2019-05-12 14:43:12
//Deal #2, Balance 10 000, Sell GBPUSD 0.2 lots at 2019-05-12 14:43:23
//Deal #3, Balance 1 000, Sell GBPUSD 1.2 lots at 2019-05-12 14:43:23
//Deal #4, Balance 10 000, Sell GBPUSD 0.21 lot at 2019-05-12 14:43:24 <- triggered match with deal #2
//Deal #5, Balance 20 000, Sell GBPUSD 0.4 lot at 2019-05-12 14:43:24 <- triggered match with deal #2 and deal #4

public class DataGeneratorTest
{
    // This holds an information or set of rules for generating fake data for TradeModel

    Faker<TradeModel> tradeModelFake;

    public DataGeneratorTest()
    {
        // This field is optional and it allows us to replicate the results (everytime we runs an application, the app will generate same results)
        Randomizer.Seed = new Random(123);
        DateTimeOffset initialTimestamp = DateTimeOffset.Now.AddDays(-10);

        tradeModelFake = new Faker<TradeModel>()
            .RuleFor(u => u.DealsGuid, f => f.Finance.Random.Guid())
            .RuleFor(u => u.Currency, f => GetRandomCurrencyPair(f)) // We are getting a lot of different currencyPairs but not the same ones . This could provide to little matches in suspisious deals
            .RuleFor(u => u.TimeStamp, (f, u) =>
            {
                initialTimestamp = initialTimestamp.AddMinutes(f.Random.Int(1, 3)); // Due to assignment, Timestamps should be closer to each other and in chronological order
                return initialTimestamp;

            })
            .RuleFor(u => u.Action, f => f.PickRandom<TradeAction>())
            .RuleFor(u => u.Lot, f => Math.Round(f.Random.Decimal(), 3)) // Lot rounded to two decimals in the example, I round to 3 just because.
            .RuleFor(u => u.AccountBalance, f => Math.Round(f.Finance.Random.Decimal(1, 10000), 3) // Rounded to 3 decimal places as well
            );

        //Math.Round(f.Random.Decimal())
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
        var fakeTrades = GenerateFakeTrades().Take(new Random().Next(20, 100));

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
