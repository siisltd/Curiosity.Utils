using Curiosity.Configuration;
using Curiosity.Hosting;
using Curiosity.SMS;
using Curiosity.SMS.Iqsms;
using EntryPoint;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

// main
var bootstrapper = new AppBootstrapper();
return await bootstrapper.RunAsync(args);

public class CliArgs : CuriosityCLIArguments
{
    [OptionParameter("phone", 'p')]
    [Help("Phone number you want to send test message")]
    public string PhoneNumber { get; set; } = null!;
}

public class Configuration : CuriosityAppConfiguration
{
    public Configuration()
    {
        AppName ??= "IQSMS sample app";
    }
    
    // must specified login and password
    public IqsmsOptions IqsmsOptions { get; set; } = new IqsmsOptions();
    
    public override IReadOnlyCollection<ConfigurationValidationError> Validate(string? prefix = null)
    {
        var errors = new ConfigurationValidationErrorCollection(prefix);
        
        errors.AddErrors(base.Validate(prefix));
        errors.AddErrors(IqsmsOptions.Validate(nameof(IqsmsOptions)));

        return errors;
    }
}

// App that send test message via iqsms.ru
// shows how to run simple app via Curiosity.
// Use it for debug iqsms sender.
public class AppBootstrapper : CuriosityToolAppBootstrapper<CliArgs, Configuration>
{
    public AppBootstrapper()
    {
        ConfigureServices((services, config) =>
        {
            // add our test service
            services.AddIqsmsSender(config.IqsmsOptions);
        });
    }
    
    // main code
    protected override async Task<int> ExecuteAsync(
        IServiceProvider serviceProvider,
        string[] rawArguments,
        CliArgs arguments,
        Configuration configuration,
        CancellationToken cancellationToken = default)
    {
        var logger = serviceProvider.GetRequiredService<ILogger>();
        var sender = serviceProvider.GetRequiredService<ISmsSender>();

        var phoneNumber = arguments.PhoneNumber ?? throw new InvalidOperationException("Phone number can't be empty!");

        // execute
        var result = await sender.SendSmsAsync(phoneNumber, "Hello iqsms!", cancellationToken);
        
        // check result
        if (result.IsSuccess)
        {
            logger.LogInformation("We have successfully sent test message!");
        }
        else
        {
            logger.LogError("We have some problems with sending test message: \"{Error}\", Content: \"{Content}\"",
                result.Errors.Select(x => $"code: {x.Code}, description: {x.Description}"),
                result.Body.ResponseJson);
        }

        return CuriosityExitCodes.Success;
    }
}