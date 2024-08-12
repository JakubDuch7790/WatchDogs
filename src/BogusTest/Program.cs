// See https://aka.ms/new-console-template for more information
using BogusTest;

IDataGeneratorTest dataGenerator = new DataGeneratorTest();
ISuspiciousDealsDetector suspiciousDealsDetector = new SuspiciousDealsDetector(dataGenerator);

Console.WriteLine("Hello, World!");
Console.WriteLine("---------------------------------");
Console.WriteLine("Press key to start");

Console.ReadKey();

var dgrbt = new DataGeneratorRepeaterBackroundTask(TimeSpan.FromMilliseconds(1000), dataGenerator);

dgrbt.Start();

Console.WriteLine("Press key to StopTask");
Console.ReadKey();

await dgrbt.StopAsync();


