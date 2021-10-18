using System;
using System.Collections.Generic;
using Curiosity.Configuration;

namespace SIISLtd.RequestProcessing.Options
{
    /// <summary>
    /// Настройки приложения-обработчика запросов из очереди.
    /// </summary>
    public abstract class RequestProcessorNodeOptions :
        ILoggableOptions,
        IValidatableOptions
    {
        private const string ShouldBeEqualOrGreaterThanMask = "Должен быть больше либо равен {0}";

        /// <summary>
        /// События, на которые надо подписаться, чтобы узнавать о новых запросах.
        /// </summary>
        public abstract IReadOnlyList<string> EventNames { get; }

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
        /// Время, через которое в случае простоя будет отправлено keep alive сообщение в БД (в сек).
        /// </summary>
        public int KeepAliveSec { get; set; } = 0;
        
        /// <summary>
        /// Время ожидания перед переподключением к БД в случае разрыва соединения.
        /// </summary>
        public int ReconnectionPauseMs { get; set; } = 100;
        
        /// <summary>
        /// Время периодической проверки базы на запросы, события от которых потерялись/не дошли.
        /// </summary>
        public int EventsPeriodicCheckSec { get; set; } = 60;

        /// <summary>
        /// Настройки фильтрации запросов.
        /// </summary>
        public FiltrationOptions Filtration { get; set; } = new FiltrationOptions();

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
            var errorsBuilder = new ConfigurationValidationErrorCollection(prefix);

            errorsBuilder.AddErrorIf(String.IsNullOrWhiteSpace(Name), nameof(Name), " не должно быть пустым");
            errorsBuilder.AddErrorIf(KeepAliveSec < 0, nameof(KeepAliveSec), String.Format(ShouldBeEqualOrGreaterThanMask, 0));
            errorsBuilder.AddErrorIf(ReconnectionPauseMs < 0, nameof(ReconnectionPauseMs), String.Format(ShouldBeEqualOrGreaterThanMask, 0));
            errorsBuilder.AddErrorIf(EventsPeriodicCheckSec < 0, nameof(EventsPeriodicCheckSec), String.Format(ShouldBeEqualOrGreaterThanMask, 0));
            errorsBuilder.AddErrorIf(WorkersCount <= 0, nameof(WorkersCount), String.Format(ShouldBeEqualOrGreaterThanMask, 1));
            errorsBuilder.AddErrorIf(StateFlushPeriodSec <= 0, nameof(StateFlushPeriodSec), String.Format(ShouldBeEqualOrGreaterThanMask, 1));

            if (Filtration != null!)
            {
                errorsBuilder.AddErrors(Filtration.Validate($"{nameof(Filtration)}"));
            }
            errorsBuilder.AddErrorIf(EventNames.Count == 0, nameof(EventNames), "Не задано ни одного события для подписки к PostgreSQL.");
            
            return errorsBuilder;
        }
    }
}
