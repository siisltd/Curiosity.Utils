using System;
using System.Collections.Generic;
using Curiosity.Configuration;

namespace Curiosity.RequestProcessing.Postgres
{
    public class PostgresEventReceiverOptions : ILoggableOptions, IValidatableOptions
    {
        private const string ShouldBeEqualOrGreaterThanMask = "Должен быть больше либо равен {0}";

        /// <summary>
        /// Время, через которое в случае простоя будет отправлено keep alive сообщение в БД (в сек).
        /// </summary>
        public int KeepAliveSec { get; set; } = 0;

        /// <summary>
        /// Время ожидания перед переподключением к БД в случае разрыва соединения.
        /// </summary>
        public int ReconnectionPauseMs { get; set; } = 100;

        /// <summary>
        /// События, на которые надо подписаться, чтобы узнавать о новых запросах.
        /// </summary>
        public IReadOnlyList<string> EventNames { get; set; } = Array.Empty<string>();

        /// <inheritdoc />
        public IReadOnlyCollection<ConfigurationValidationError> Validate(string? prefix = null)
        {
            var errors = new ConfigurationValidationErrorCollection(prefix);

            errors.AddErrorIf(KeepAliveSec < 0, nameof(KeepAliveSec), String.Format(ShouldBeEqualOrGreaterThanMask, 0));
            errors.AddErrorIf(ReconnectionPauseMs < 0, nameof(ReconnectionPauseMs), String.Format(ShouldBeEqualOrGreaterThanMask, 0));
            errors.AddErrorIf(EventNames.Count == 0, nameof(EventNames), "Не задано ни одного события для подписки к PostgreSQL.");

            return errors;
        }
    }
}
