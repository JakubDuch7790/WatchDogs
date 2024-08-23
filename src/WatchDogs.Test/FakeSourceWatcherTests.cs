using Contracts;
using Infrastructure.DxTrade;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WatchDogs.Contracts;
using WatchDogs.Infrastructure.FakeSource;

namespace WatchDogs.Test;

public class FakeSourceWatcherTests
{
    private readonly FakeSourceWatcher _fakeSourceWatcher;
    private readonly Mock<IFakeTradeGenerator> _fakeTradeGenerator = new Mock<IFakeTradeGenerator>();
    private readonly Mock<IDataInserter> _dataInserter = new Mock<IDataInserter>();
    public FakeSourceWatcherTests()
    {
        var nullLogger = NullLogger<FakeSourceWatcher>.Instance;

        var optionsMock = new Mock<IOptions<FakeSourceOptions>>();

        var connectionOptions = new FakeSourceOptions
        {
            IntervalInMilliseconds = 1000
        };

        optionsMock.Setup(o => o.Value).Returns(connectionOptions);

        _fakeSourceWatcher = new(_fakeTradeGenerator.Object, _dataInserter.Object, nullLogger, optionsMock.Object);
    }
    [Fact]
    public async Task StartAsync_StartProcessOfLoadingData_InCaseOfValidCancelletationToken()
    {
        //Arrange
        //Act
        await _fakeSourceWatcher.StartAsync();
        //Assert
    }
    [Fact]
    public async Task StartAsync_StopProcessOfLoadingData_InCaseOfCancelletationTokenRequest()
    {
        //Arrange
        //Act
        //Assert
    }
    [Fact]
    public async Task StartAsync_ThrowsAnException_InCaseOfSomeError()
    {
        //Arrange
        //Act
        //Assert
    }


}
