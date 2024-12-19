using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WatchDogs.Contracts;
using WatchDogs.Persistence.EntityFramework;

namespace WatchDogs.Test;
public class TradeHandlerTests
{
    private ApplicationDbContext CreateInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase("TestDatabase")
            .Options;
        return new ApplicationDbContext(options);
    }

    private readonly TradeHandler _tradeHandler;

    public TradeHandlerTests()
    {
        _tradeHandler = new TradeHandler(CreateInMemoryDbContext());
    }

    [Fact]
    public async Task HandleTradeAsync_ShouldRemoveProcessedTrade()
    {
        //Arrange
        Trade trade = new Trade();

        //Act
        _tradeHandler.HandleTradeAsync(trade);

        //Assert
    }

}
