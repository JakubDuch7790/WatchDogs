using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Moq;
using WatchDogs.Contracts;
using WatchDogs.Infrastructure.FakeSource;
using WatchDogs.Persistence.EntityFramework;

namespace WatchDogs.Test;
public class TradeLoaderTests
{
    private ApplicationDbContext CreateInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase("TestDatabase")
            .Options;
        return new ApplicationDbContext(options);
    }

    private readonly TradeLoader _tradeLoader;

    private readonly ApplicationDbContext _context;

    private readonly FakeTradeGenerator _tradeGenerator;
    private readonly IOptions<FakeTradegeneratorOptions> _fakeGeneratorOptions;

    public TradeLoaderTests()
    {
        var fakeSourceOptions = new FakeTradegeneratorOptions { GeneratedTradesTop = 10, GeneratedTradesBottom = 10 };
        _fakeGeneratorOptions = Options.Create(fakeSourceOptions);

        _context = CreateInMemoryDbContext();

        _tradeLoader = new TradeLoader(_context);
        _tradeGenerator = new FakeTradeGenerator(_fakeGeneratorOptions);
    }
    [Fact]
    public async Task LoadOneTradeAtTimeAsync_Should___SelfExplanatory()
    {
        //Arrange
        var trades = new List<Trade>();
        trades.AddRange(_tradeGenerator.LoadFakeData());
        //_tradeLoader._lastLoadedTradeIdd = trades.FirstOrDefault().Id;

        //Act

        var loadedTrade = _tradeLoader.LoadOneTradeAtTimeAsync();

        //Assert
        Assert.Equal(await loadedTrade ,trades.First());
    }
}
