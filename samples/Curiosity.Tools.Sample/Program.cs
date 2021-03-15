using System;
using System.Threading.Tasks;
using Curiosity.Tools.Performance;

namespace Curiosity.Tools.Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine($"\nResult of {nameof(SensitiveDataProtectorSample)}:");
            SensitiveDataProtectorSample.Run();
            
            Console.WriteLine($"\nResult of {nameof(TransliterationSample)}:");
            TransliterationSample.Run();
            
            Console.WriteLine($"\nResult of {nameof(PhoneHelperSample)}:");
            PhoneHelperSample.Run();


            using (PerformanceManager.Measure("Test"))
            {
                Task.Delay(TimeSpan.FromSeconds(1));
            }

            using (StuckCodeManager.Enter("Stuck code", 1))
            {
                Task.Delay(TimeSpan.FromSeconds(2));
            }
        }
    }
}