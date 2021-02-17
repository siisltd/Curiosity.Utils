using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Marvin.Hosting.Web.IntegrationTests
{
    public class MarvinWebAppBootstrapper_Should
    {
        
        
        public class Startup
        {
            public void ConfigureServices(IServiceCollection services)
            {
            }

            public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
            {
                app.Use((context, func) => context.Response.WriteAsync("Hello world"));
            }
        }
        
        [Fact]
        public async Task Bootstrap_Correctly()
        {
            var cts = new CancellationTokenSource();

            var bootstrapper = new MarvinWebAppBootstrapper<CliArgs, Configuration, Startup>();

            var task = bootstrapper.RunAsync(Array.Empty<string>(), cts.Token);
            await Task.Delay(TimeSpan.FromSeconds(10));
            
            cts.Cancel();

            var result = await task;

            result.Should().Be(0,"because it's normal execution");
        }
    }
}