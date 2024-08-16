using Serilog.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Serilog;

namespace WatchDogs.FakeSource;

public class Watcher : IWatcher
{
    private Task? _timerTask;
    private readonly PeriodicTimer _timer;
    private readonly CancellationTokenSource _cts = new();
    private readonly IFakeTradeGenerator _dataGenerator;
    private readonly ILogger _logger;



    public Watcher(TimeSpan interval, IFakeTradeGenerator dataGenerator)
    {
        _timer = new PeriodicTimer(interval);
        _dataGenerator = dataGenerator;
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
                _dataGenerator.LoadFakeData();
            }
        }
        catch (OperationCanceledException) 
        {
            _logger.Error("Something went wrong.");
        }
    }
}
