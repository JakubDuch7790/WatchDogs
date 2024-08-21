using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bogus;
using Microsoft.Extensions.Options;
using WatchDogs.Contracts;

namespace WatchDogs.Infrastructure.FakeSource;

//Deal #1, Balance 10 000, Buy EURUSD 1 lot at 2019-05-12 14:43:12
//Deal #2, Balance 10 000, Sell GBPUSD 0.2 lots at 2019-05-12 14:43:23
//Deal #3, Balance 1 000, Sell GBPUSD 1.2 lots at 2019-05-12 14:43:23
//Deal #4, Balance 10 000, Sell GBPUSD 0.21 lot at 2019-05-12 14:43:24 <- triggered match with deal #2
//Deal #5, Balance 20 000, Sell GBPUSD 0.4 lot at 2019-05-12 14:43:24 <- triggered match with deal #2 and deal #4

public class FakeTradeGenerator : IFakeTradeGenerator
{
    // This holds an information or set of rules for generating fake data for Trade

    Faker<Trade> tradeModelFake;

    private readonly FakeTradegeneratorOptions _fakeTradegeneratorOptions;

    private readonly string[] currencies =
    [
            "USD", // United States Dollar
            "EUR", // Euro
            "JPY", // Japanese Yen
            "GBP", // British Pound Sterling
            "AUD", // Australian Dollar
            "CHF", // Swiss Franc
            "CNY", // Chinese Yuan
    ];

    public FakeTradeGenerator(IOptions<FakeTradegeneratorOptions> options)
    {
        /// This field is optional and it allows us to replicate the results (everytime we runs an application, the app will generate same results)
        ///Randomizer.Seed = new Random(123);
        
        _fakeTradegeneratorOptions = options.Value;

        DateTimeOffset initialTimestamp = DateTimeOffset.Now;

        tradeModelFake = new Faker<Trade>()
            .RuleFor(u => u.Id, f => f.Finance.Random.Guid())
            .RuleFor(u => u.Currency, GetRandomCurrencyPair)
            .RuleFor(u => u.TimeStamp, (f, u) =>
            {
                initialTimestamp = initialTimestamp.AddMilliseconds(f.Random.Int(0, 5000)); // Due to assignment, Timestamps should be closer to each other and in chronological order
                return initialTimestamp;
            })
            .RuleFor(u => u.Action, f => f.PickRandom<TradeAction>())
            .RuleFor(u => u.Lot, f => Math.Round(f.Random.Decimal(), 2))
            .RuleFor(u => u.AccountBalance, f => Math.Round(f.Finance.Random.Decimal(1, 10000), 2)
            );
    }

    public IEnumerable<Trade> LoadFakeData()
    {
        List<Trade> fakeDealsList = new List<Trade>();

        var fakeTrades = tradeModelFake.GenerateForever().Take(new Random().Next(_fakeTradegeneratorOptions.GeneratedTradesBottom, _fakeTradegeneratorOptions.GeneratedTradesTop));

        foreach (var trade in fakeTrades)
        {
            fakeDealsList.Add(trade);
        }

        return fakeDealsList;
    }

    private string GetRandomCurrencyPair(Faker f)
    {
        string currency1 = f.Random.ListItem(currencies);
        string currency2 = f.Random.ListItem(currencies);

        while (currency1 == currency2)
        {
            currency2 = f.Random.ListItem(currencies);
        }

        return currency1 + currency2;
    }
}