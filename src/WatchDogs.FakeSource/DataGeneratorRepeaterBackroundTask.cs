using Serilog.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Serilog;

namespace WatchDogs.FakeSource;

public class DataGeneratorRepeaterBackroundTask : IDataGeneratorRepeaterBackroundTask
{
    private Task? _timerTask;
    private readonly PeriodicTimer _timer;
    private readonly CancellationTokenSource _cts = new();
    private readonly IFakeTradeGenerator _dataGenerator;
    private readonly ILogger _logger;



    public DataGeneratorRepeaterBackroundTask(TimeSpan interval, IFakeTradeGenerator dataGenerator, ILogger loger)
    {
        _timer = new PeriodicTimer(interval);
        _dataGenerator = dataGenerator;
        _logger = loger;
    }

    public async Task StartAsync()
    {
        _timerTask = LoadFakeDataEverySecondAsync();
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

    public async Task StopAsync()
    {
        if (_timerTask is null)
        {
            return;
        }
        _cts.Cancel();
        await _timerTask;
        _cts.Dispose();
    }
}
