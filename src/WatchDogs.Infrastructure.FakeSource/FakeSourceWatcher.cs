using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WatchDogs.Contracts;
using WatchDogs.Persistence.EntityFramework;

namespace WatchDogs.Infrastructure.FakeSource;

public class FakeSourceWatcher : IWatcher
{
    private Task? _timerTask;
    private readonly PeriodicTimer _timer;
    private readonly CancellationTokenSource _cts = new();
    private readonly IFakeTradeGenerator _dataGenerator;
    private readonly ILogger<FakeSourceWatcher> _logger;
    private readonly FakeSourceOptions _fakeSourceOptions;
    private readonly DbContextOptions<ApplicationDbContext> _dbContextOptions;
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;

    public FakeSourceWatcher(DbContextOptions<ApplicationDbContext> dbContextOptions,
        IFakeTradeGenerator dataGenerator, ILogger<FakeSourceWatcher> logger,
        IOptions<FakeSourceOptions> fakeSourceOptions, IUnitOfWorkFactory unitOfWorkFactory)
    {
        _fakeSourceOptions = fakeSourceOptions.Value;
        _timer = new PeriodicTimer(TimeSpan.FromMilliseconds(_fakeSourceOptions.IntervalInMilliseconds));
        _dataGenerator = dataGenerator;
        _logger = logger;
        _dbContextOptions = dbContextOptions;
        _unitOfWorkFactory = unitOfWorkFactory;

    }

    public async Task StartAsync(CancellationToken token = default)
    {
        try
        {
            if (!token.IsCancellationRequested)
            {
                _timerTask = LoadFakeDataEverySecondAsync();
            }
            else
            {
                if(_timerTask is not null)
                {
                    _cts.Cancel();
                    await _timerTask;
                    _cts.Dispose();
                }
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogError("Cancelled");
        }
    }

    private async Task LoadFakeDataEverySecondAsync()
    {
        using var unitOfWork = _unitOfWorkFactory.Create();

        try
        {
            while (await _timer.WaitForNextTickAsync(_cts.Token))
            {
                _logger.LogInformation("Fake data are about to be created.");

                var tradesToInsert = _dataGenerator.LoadFakeData();

                _logger.LogInformation("Fake data have been created successfully.");

                _logger.LogInformation("Fake data are about to be inserted into Db.");

                await unitOfWork.DataInserter.InsertTradeDatatoDbAsync(tradesToInsert);

                try
                {
                    await unitOfWork.SaveAsync();

                    if (_cts.Token.IsCancellationRequested)
                    {
                        _logger.LogInformation("Cancellation requested.");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"{ex.Message}");
                }

                _logger.LogInformation("Fake data have been inserted into Db successfully.");
            }
        }
        catch (OperationCanceledException) 
        {
            _logger.LogError("Something went wrong.");
        }
    }
}
