using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Curiosity.Configuration;
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

        public override IReadOnlyCollection<ConfigurationValidationError> Validate(string? prefix = null)
        {
            Console.WriteLine("demo");
            return base.Validate(prefix);
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