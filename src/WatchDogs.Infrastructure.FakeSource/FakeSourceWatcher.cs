using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WatchDogs.Contracts;

namespace WatchDogs.Infrastructure.FakeSource;

public class FakeSourceWatcher : IWatcher
{
    private Task? _timerTask;
    private readonly PeriodicTimer _timer;
    private readonly CancellationTokenSource _cts = new();
    private readonly IFakeTradeGenerator _dataGenerator;
    private readonly IDataInserter _dataInserter;
    private readonly ILogger<FakeSourceWatcher> _logger;
    private readonly FakeSourceOptions _fakeSourceOptions;

    public FakeSourceWatcher(IFakeTradeGenerator dataGenerator, IDataInserter dataInserter, ILogger<FakeSourceWatcher> logger, IOptions<FakeSourceOptions> fakeSourceOptions)
    {
        _fakeSourceOptions = fakeSourceOptions.Value;

        _timer = new PeriodicTimer(TimeSpan.FromMilliseconds(_fakeSourceOptions.IntervalInMilliseconds));
        _dataGenerator = dataGenerator;
        _dataInserter = dataInserter;
        _logger = logger;
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
        try
        {
            while (await _timer.WaitForNextTickAsync(_cts.Token))
            {
                _logger.LogInformation("Fake data are about to be created.");
                var tradesToInsert = _dataGenerator.LoadFakeData();
                _logger.LogInformation("Fake data have been created successfully.");

                _logger.LogInformation("Fake data are about to be inserted into Db.");
                await _dataInserter.InsertTradeDatatoDbAsync(tradesToInsert);
                _logger.LogInformation("Fake data have been inserted into Db successfully.");

            }
        }
        catch (OperationCanceledException) 
        {
            _logger.LogError("Something went wrong.");
        }
    }
}
