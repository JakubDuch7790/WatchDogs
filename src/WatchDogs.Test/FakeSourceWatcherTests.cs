using Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System.Threading.Tasks;
using WatchDogs.Contracts;
using WatchDogs.Infrastructure.FakeSource;
using WatchDogs.Persistence.EntityFramework;

namespace WatchDogs.Test
{
    public class FakeSourceWatcherTests
    {
        private readonly Mock<ILogger<FakeSourceWatcher>> _mockLogger;
        private readonly Mock<IUnitOfWorkFactory> _unitOfWorkFactory;
        private readonly Mock<IFakeTradeGenerator> _fakeTradeGenerator;
        private readonly DbContextOptions<ApplicationDbContext> _dbContext;
        private readonly IOptions<FakeSourceOptions> _fakeSourceOptions;
        private readonly CancellationTokenSource _cts = new(TimeSpan.FromSeconds(5));

        private readonly FakeSourceWatcher _watcher;

        public FakeSourceWatcherTests()
        {
            _mockLogger = new Mock<ILogger<FakeSourceWatcher>>();
            _unitOfWorkFactory = new Mock<IUnitOfWorkFactory>();
            _fakeTradeGenerator = new Mock<IFakeTradeGenerator>();

            var fakeSourceOptions = new FakeSourceOptions { IntervalInMilliseconds = 1000 };
            _fakeSourceOptions = Options.Create(fakeSourceOptions);


            var mockUnitOfWorkFactory = new Mock<IUnitOfWorkFactory>();
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            var mockTradeInserter = new Mock<ITradeInserter>();

            mockUnitOfWork.Setup(x => x.DataInserter).Returns(mockTradeInserter.Object);

            mockTradeInserter.Setup(x => x.InsertTradeDatatoDbAsync(It.IsAny<IEnumerable<Trade>>())).Returns(Task.CompletedTask);

            mockUnitOfWorkFactory.Setup(f => f.Create()).Returns(mockUnitOfWork.Object);

            _watcher = new FakeSourceWatcher(_dbContext, _fakeTradeGenerator.Object, _mockLogger.Object,
                _fakeSourceOptions, mockUnitOfWorkFactory.Object);
        }

        [Fact]
        public async Task StartAsync_ShouldStart_timerTaskWhenNoCancellationIsRequested()
        {
            //Arrange
            CancellationToken token = _cts.Token;

            CancellationTokenSource ctssss = new CancellationTokenSource(TimeSpan.FromSeconds(5));

            //Act
            var SS = _watcher.StartAsync(ctssss.Token);

            //Assert
            
        }
        [Fact]
        public async Task StartAsync_ShouldStopFunctionWhenCancellationIsRequested()
        {
            //Arrange
            CancellationTokenSource ctssss = new CancellationTokenSource(TimeSpan.FromSeconds(5));

            //Act
            var task = _watcher.StartAsync(ctssss.Token);

            var Carinhall = task.IsCanceled;

            //Assert
            Assert.True(Carinhall);
            await Assert.ThrowsAsync<OperationCanceledException>((async () => await task));
        }

    }
}
