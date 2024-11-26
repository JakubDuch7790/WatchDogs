using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using WatchDogs.Contracts;
using WatchDogs.Infrastructure.FakeSource;
using WatchDogs.Persistence.EntityFramework;

namespace WatchDogs.Test
{
    public class FakeSourceWatcherTests
    {
        private readonly DbContextOptions<ApplicationDbContext> _dbContext;
        private readonly IOptions<FakeSourceOptions> _fakeSourceOptions;

        private readonly Mock<ILogger<FakeSourceWatcher>> _mockLogger;
        private readonly Mock<IFakeTradeGenerator> _fakeTradeGenerator;
        private readonly Mock<IUnitOfWorkFactory> _unitOfWorkFactory;

        private readonly CancellationTokenSource _cts = new();
        private readonly FakeSourceWatcher _watcher;

        public FakeSourceWatcherTests()
        {
            _mockLogger = new Mock<ILogger<FakeSourceWatcher>>();
            _unitOfWorkFactory = new Mock<IUnitOfWorkFactory>();
            _fakeTradeGenerator = new Mock<IFakeTradeGenerator>();

            var fakeSourceOptions = new FakeSourceOptions { IntervalInMilliseconds = 1000 };
            _fakeSourceOptions = Options.Create(fakeSourceOptions);

            var mockUnitOfWork = new Mock<IUnitOfWork>();
            var mockTradeInserter = new Mock<ITradeInserter>();

            mockUnitOfWork.Setup(x => x.DataInserter).Returns(mockTradeInserter.Object);
            mockTradeInserter.Setup(x => x.InsertTradeDatatoDbAsync(It.IsAny<IEnumerable<Trade>>())).Returns(Task.CompletedTask);
            _unitOfWorkFactory.Setup(f => f.Create()).Returns(mockUnitOfWork.Object);

            _watcher = new FakeSourceWatcher(_dbContext, _fakeTradeGenerator.Object, _mockLogger.Object,
                _fakeSourceOptions, _unitOfWorkFactory.Object);
        }

        [Fact]
        public async Task StartAsync_ShouldStart_timerTaskWhenNoCancellationIsRequested()
        {
            //Arrange
            CancellationToken token = _cts.Token;
            _cts.CancelAfter(1000);

            //Act
            await _watcher.StartAsync(token);

            //Assert

            //Internet advised that it could be testet indirectly via logger.
            //I've really tried so hard to do it differently but in the end I had to used this workaround.

            //StackOverflow below, kein GPT
            _mockLogger.Verify(
                x => x.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)));

            // this was causing error Unsupported expression even though in Nick's video he had it exactly same
            //_mockLogger.Verify(l => l.LogInformation("Task sucessfully assigned."), Times.Once);
        }
        [Fact]
        public async Task StartAsync_ShouldStopFunctionWhenCancellationIsRequested()
        {
            //Arrange

            var cts = new CancellationTokenSource();
            cts.Cancel(); // Precancelled  token

            //Act und Assert

            await Assert.ThrowsAsync<OperationCanceledException>(() => _watcher.StartAsync(cts.Token));
        }
    }
}
