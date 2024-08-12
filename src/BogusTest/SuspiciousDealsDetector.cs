using Bogus.DataSets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BogusTest;

public class SuspiciousDealsDetector : ISuspiciousDealsDetector
{
    /* Similar deals are deals where: 
     * open time differs no more than one second,
     * currency pair is the same and
     * the difference in volume-to-balance ratio is no more than 5%. */

    // open time differs
    private const int TimeDifferenceBetweenDeals = 1; 
    public TimeSpan timeSpan = TimeSpan.FromSeconds(1); // maybe rather this, we'll see

    //System.Threading.Timer
    private Timer timer;

    IDataGeneratorTest dataGenerator = new DataGeneratorTest();

    public SuspiciousDealsDetector(IDataGeneratorTest dataGenerator)
    {
        this.dataGenerator = dataGenerator;
    }

    public void CalculateFakeDeal()
    {
        timer = new Timer(TimerCallbackAKATick, null, 1000, 1000);

    }

    private void TimerCallbackAKATick(object sender)
    {
        
        
        var fakedeals = dataGenerator.LoadFakeData();


    }

    //public void StartTimer()
    //{
    //    timer = new Timer(TimerCallbackWrapperDacoDaco, null, 1000, 1000);
    //}

    public void StopTimer()
    {
        timer?.Dispose();
    }







}
