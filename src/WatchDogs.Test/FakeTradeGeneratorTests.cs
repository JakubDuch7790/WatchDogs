using Contracts;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WatchDogs.Infrastructure.FakeSource;

namespace WatchDogs.Test;
public class FakeTradeGeneratorTests
{
    private readonly FakeTradeGenerator _generator;
    public FakeTradeGeneratorTests()
    {
        var optionsMock = new Mock<IOptions<FakeTradegeneratorOptions>>();

        var connectionOptions = new FakeTradegeneratorOptions
        {
            GeneratedTradesTop = 50,
            GeneratedTradesBottom = 5
        };

        optionsMock.Setup(o => o.Value).Returns(connectionOptions);

        _generator = new FakeTradeGenerator(optionsMock.Object);

    }
    [Fact]
    public void LoadFakeData_ShouldLoadDataInRangeProvidedByConfig()
    {
        //Arrange

        FakeTradeGenerator fake = _generator;

        int low = 5;
        int high = 50;

        //Act

        var generatedData = fake.LoadFakeData();

        //Assert

        Assert.InRange<int>(generatedData.Count(), low, high);
    }
}
