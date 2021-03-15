using System;

namespace Curiosity.Tools.Sample
{
    public static class PhoneHelperSample
    {
        public static void Run()
        {
            var phone = "+7(900)-123-45-67";
            var cleaned = phone.CleanupPhone();
            var to8Format = phone.ResolvePhoneTo8Format();
            var to7Format = to8Format.ResolvePhoneTo7Format();
            var trimmed = phone.Remove7And8();
            var isMobileFormat = cleaned.IsMobilePhoneFormat();
            
            Console.WriteLine($"Original phone: {phone}");
            Console.WriteLine($"Cleaned: {cleaned}");
            Console.WriteLine($"To 8 format: {to8Format}");
            Console.WriteLine($"To 7 format: {to7Format}");
            Console.WriteLine($"Trimmed: {trimmed}");
            Console.WriteLine($"Is it mobile format: {isMobileFormat}");
        }
    }
}