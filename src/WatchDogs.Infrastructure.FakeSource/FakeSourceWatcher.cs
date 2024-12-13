using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Threading;
using WatchDogs.Contracts;
using WatchDogs.Persistence.EntityFramework;

namespace WatchDogs.Infrastructure.FakeSource;

public class FakeSourceWatcher : IWatcher
{
    private Task? _timerTask;
    private readonly PeriodicTimer _timer;
    private readonly IFakeTradeGenerator _dataGenerator;
    private readonly ILogger<FakeSourceWatcher> _logger;
    private readonly FakeSourceOptions _fakeSourceOptions;
    private readonly DbContextOptions<ApplicationDbContext> _dbContextOptions;
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;

    public FakeSourceWatcher(DbContextOptions<ApplicationDbContext> dbContextOptions,
        IFakeTradeGenerator dataGenerator, ILogger<FakeSourceWatcher> logger,
        IOptions<FakeSourceOptions> fakeSourceOptions, IUnitOfWorkFactory unitOfWorkFactory, Task? task = null)
    {
        _fakeSourceOptions = fakeSourceOptions.Value;
        _timer = new PeriodicTimer(TimeSpan.FromMilliseconds(_fakeSourceOptions.IntervalInMilliseconds));
        _dataGenerator = dataGenerator;
        _logger = logger;
        _dbContextOptions = dbContextOptions;
        _unitOfWorkFactory = unitOfWorkFactory;
        _timerTask = task;
    }

    public async Task StartAsync(CancellationToken token = default)
    {
        try
        {
            token.ThrowIfCancellationRequested();

            while (!token.IsCancellationRequested)
            {

                _timerTask = LoadFakeDataEverySecondAsync(token);

                if(_timerTask is not null)
                {
                    _logger.LogInformation("Task sucessfully assigned.");
                }

                await _timerTask;
            }
        }
         
        catch (OperationCanceledException ex)
        {
            _logger.LogError("Cancelled");

            throw;
        }
        // code before that did not work

        //catch (OperationCanceledException)
        //{
        //    _logger.LogError("Cancelled");
        //}


        finally
        {
            if( _timerTask is not null)
            {
                try
                {
                    await _timerTask;
                }
                catch (OperationCanceledException)
                {
                    _logger.LogError("Operation cancelled.");
                }
            }
        }
    }

    private async Task LoadFakeDataEverySecondAsync(CancellationToken token)
    {
        using var unitOfWork = _unitOfWorkFactory.Create();

        try
        {
            while (await _timer.WaitForNextTickAsync(token))
            {
                _logger.LogInformation("Fake data are about to be created.");

                var tradesToInsert = _dataGenerator.LoadFakeData();

                _logger.LogInformation("Fake data have been created successfully.");

                _logger.LogInformation("Fake data are about to be inserted into Db.");
                
                await unitOfWork.TradeInserter.InsertAsync(tradesToInsert);


                try
                {
                    await unitOfWork.SaveAsync();

                    if (token.IsCancellationRequested)
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
