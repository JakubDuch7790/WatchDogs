using Serilog.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Serilog;
using WatchDogs.Persistence.EntityFramework;

namespace WatchDogs.Infrastructure.FakeSource;

public class Watcher : IWatcher
{
    private Task? _timerTask;
    private readonly PeriodicTimer _timer;
    private readonly CancellationTokenSource _cts = new();
    private readonly IFakeTradeGenerator _dataGenerator;
    private readonly ILogger _logger;
    private readonly DataInserter _dataInserter;



    public Watcher(TimeSpan interval, IFakeTradeGenerator dataGenerator, DataInserter dataInserter)
    {
        _timer = new PeriodicTimer(interval);
        _dataGenerator = dataGenerator;
        _dataInserter = dataInserter;
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
            _logger.Error("Cancelled");
        }
    }

    private async Task LoadFakeDataEverySecondAsync()
    {
        try
        {
            while (await _timer.WaitForNextTickAsync(_cts.Token))
            {
                var tradesToInsert = _dataGenerator.LoadFakeData();

                await _dataInserter.InsertTradeDatatoDbAsync(tradesToInsert);


            }
        }
        catch (OperationCanceledException) 
        {
            _logger.Error("Something went wrong.");
        }
    }
}
