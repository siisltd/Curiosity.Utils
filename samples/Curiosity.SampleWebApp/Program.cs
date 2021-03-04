using System.Threading.Tasks;
using Curiosity.Hosting;
using Curiosity.Hosting.Web;

namespace Curiosity.SampleWebApp
{
    public class Configuration : CuriosityWebAppConfiguration
    {
        public Configuration()
        {
            AppName = "Test App";
        }
    }

    public class Program
    {
        public class CliArgs : CuriosityCLIArguments
        {
            public CliArgs() : base("test")
            {
            }
        }

        public static Task<int> Main(string[] args)
        {
            var bootstrapper = new CuriosityWebAppBootstrapper<CliArgs, Configuration, Startup>();

            return bootstrapper.RunAsync(args);
        }
    }
}