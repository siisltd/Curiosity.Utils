using System;

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
            
        }
    }
}