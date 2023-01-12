using System.Collections.Generic;
using Curiosity.Configuration;

namespace Curiosity.DateTime.DbSync
{
    /// <summary>
    /// Options for configuration date time service that syncs time with database.
    /// </summary>
    public class DbDateTimeOptions : 
        IValidatableOptions,
        ILoggableOptions
    {
        /// <summary>
        /// Period of synchronization time with database, minutes.
        /// </summary>
        public int SyncPeriodMin { get; set; } = 30;

        /// <inheritdoc />
        public virtual IReadOnlyCollection<ConfigurationValidationError> Validate(string? prefix = null)
        {
            var errors = new ConfigurationValidationErrorCollection(prefix);

            errors.AddErrorIf(SyncPeriodMin <= 0, nameof(SyncPeriodMin), "should be greater than 0");

            return errors;
        }
    }
}