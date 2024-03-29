using System;
using System.Threading;
using System.Threading.Tasks;
using Curiosity.Tools;
using Curiosity.Tools.AppInitializer;
using Microsoft.Extensions.Logging;

namespace Curiosity.DateTime.DbSync
{
    /// <summary>
    /// Initializer of <see cref="DbSyncDateTimeService"/>.
    /// </summary>
    public class DateTimeServiceInitializer<T>
        : IAppInitializer
        where T: DbSyncDateTimeService
    {
        private readonly DbSyncDateTimeService _dateTimeService;
        private readonly ILogger _logger;

        public DateTimeServiceInitializer(
            T dateTimeService,
            ILogger<DateTimeServiceInitializer<T>> logger)
        {
            _dateTimeService = dateTimeService ?? throw new ArgumentNullException(nameof(dateTimeService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <inheritdoc />
        public async Task InitializeAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogDebug("Initialization of date and time service started...");
            await _dateTimeService.InitAsync(cancellationToken);
            _logger.LogDebug("Initialization of date and time service completed.");
        }
    }
}
