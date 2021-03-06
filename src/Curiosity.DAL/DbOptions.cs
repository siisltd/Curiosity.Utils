using System;
using System.Collections.Generic;
using Curiosity.Configuration;

namespace Curiosity.DAL
{
    /// <summary>
    /// Options for database connection.
    /// </summary>
    /// <remarks>
    /// Like connection string but with more extra info.
    /// </remarks>
    public class DbOptions : ILoggableOptions, IValidatableOptions
    {
        private const string ConnectionStringCanNotBeEmpty = "can't be empty";

        /// <summary>
        /// Connection string for read-write access.
        /// </summary>
        public string ConnectionString { get; set; } = null!;

        /// <summary>
        /// Connection string for read-only access.
        /// </summary>
        public string? ReadOnlyConnectionString { get; set; }

        /// <summary>
        /// Option for logging all SQL queries.
        /// </summary>
        public bool IsGlobalLoggingEnabled { get; set; }
        
        /// <summary>
        /// Option for logging sensitive data of SQL queries (mostly query params).
        /// </summary>
        public bool IsSensitiveDataLoggingEnabled { get; set; }

        /// <inheritdoc />
        public virtual IReadOnlyCollection<ConfigurationValidationError> Validate(string? prefix = null)
        {
            var errors = new ConfigurationValidationErrorCollection(prefix);

            if (String.IsNullOrWhiteSpace(ConnectionString))
            {
                errors.AddError(nameof(ConnectionString), ConnectionStringCanNotBeEmpty);
            }

            return errors;
        }
    }
}