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
    public FakeTradeGeneratorTests()
    {

    }

    [Theory]
    [InlineData(5, 50)]
    [InlineData(1, 10)]
    [InlineData(20, 30)]
    [InlineData(0, 2)]
    public void LoadFakeData_ShouldLoadDataInRangeProvidedByConfig(int bottom, int top)
    {
        //Arrange
        var optionsMock = new Mock<IOptions<FakeTradegeneratorOptions>>();

        var connectionOptions = new FakeTradegeneratorOptions
        {
            GeneratedTradesTop = top,
            GeneratedTradesBottom = bottom
        };

        optionsMock.Setup(o => o.Value).Returns(connectionOptions);

        FakeTradeGenerator fake = new(optionsMock.Object);

        //Act
        var generatedData = fake.LoadFakeData();

        //Assert
        Assert.InRange<int>(generatedData.Count(), bottom, top);
    }
}
