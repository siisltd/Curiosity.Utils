namespace Curiosity.Tools
{
    public static class EnvironmentHelper
    {
        public static bool IsDevelopmentEnvironment()
        {
            return Equals(System.Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")?.ToLower(), "development");
        }

        public static bool IsProductionEnvironment()
        {
            return Equals(System.Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")?.ToLower(), "production");
        }
    }
}