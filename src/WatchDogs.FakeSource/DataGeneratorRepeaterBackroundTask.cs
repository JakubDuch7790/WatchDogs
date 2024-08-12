using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WatchDogs.FakeSource;

public class DataGeneratorRepeaterBackroundTask : IDataGeneratorRepeaterBackroundTask
{
    private Task? _timerTask;
    private readonly PeriodicTimer _timer;
    private readonly CancellationTokenSource _cts = new();
    private readonly IDataGenerator _dataGenerator;


    public DataGeneratorRepeaterBackroundTask(TimeSpan interval, IDataGenerator dataGenerator)
    {
        _timer = new PeriodicTimer(interval);
        _dataGenerator = dataGenerator;
    }

    public void Start()
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
        catch (OperationCanceledException) { }
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
