using System;
using System.Collections.Generic;
using Curiosity.Configuration;

namespace Curiosity.RequestProcessing
{
    /// <summary>
    /// Настройки приложения-обработчика запросов из очереди.
    /// </summary>
    public class RequestProcessorNodeOptions :
        ILoggableOptions,
        IValidatableOptions
    {
        private const string ShouldBeEqualOrGreaterThanMask = "Должен быть больше либо равен {0}";

        /// <summary>
        /// Уникальное имя обработчика запросов из очереди.
        /// </summary>
        /// <remarks>
        /// Используется для различия нод, которые взяли запрос в обработку при параллельной работе
        /// (каждая нода, когда берет запрос, лочит его, указывая свое имя).
        /// Например, ISACR-1, ISACR-WCIOM.
        /// </remarks>
        public string Name { get; set; } = null!;

        /// <summary>
        /// Время периодической проверки базы на запросы, события от которых потерялись/не дошли.
        /// </summary>
        public int EventsPeriodicCheckSec { get; set; } = 60;

        /// <summary>
        /// Количество воркеров.
        /// </summary>
        public int WorkersCount { get; set; }

        /// <summary>
        /// Период для записи в лог состояния диспетчера и воркеров в лог, секунды.
        /// </summary>
        public int StateFlushPeriodSec { get; set; } = 60;

        /// <inheritdoc />
        public virtual IReadOnlyCollection<ConfigurationValidationError> Validate(string? prefix = null)
        {
            var errors = new ConfigurationValidationErrorCollection(prefix);

            errors.AddErrorIf(String.IsNullOrWhiteSpace(Name), nameof(Name), " не должно быть пустым");
            errors.AddErrorIf(EventsPeriodicCheckSec < 0, nameof(EventsPeriodicCheckSec), String.Format(ShouldBeEqualOrGreaterThanMask, 0));
            errors.AddErrorIf(WorkersCount <= 0, nameof(WorkersCount), String.Format(ShouldBeEqualOrGreaterThanMask, 1));
            errors.AddErrorIf(StateFlushPeriodSec <= 0, nameof(StateFlushPeriodSec), String.Format(ShouldBeEqualOrGreaterThanMask, 1));

            return errors;
        }
    }
}
