using Curiosity.Configuration;

namespace Curiosity.RequestProcessing.Postgres
{
    public interface IPostgresRequestProcessorNodeOptions: ILoggableOptions, IValidatableOptions
    {
        PostgresEventReceiverOptions PostgresEventReceiver { get; }
    }
}
