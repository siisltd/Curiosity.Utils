using System;
using System.Text.Json;

namespace Curiosity.Tools.Sample
{
    public static class SensitiveDataProtectorSample
    {
        public static void Run()
        {
            var protector = new SensitiveDataProtector("password");

            var json = JsonSerializer.Serialize(new
            {
                Login = "Login",
                Password = "Super secret password",
                OtherField = "Other data",
            });

            var protectedJson = protector.HideInJson(json);
            
            Console.WriteLine($"Original json: {json}");
            Console.WriteLine($"Protected json: {protectedJson}");
        }
    }
}