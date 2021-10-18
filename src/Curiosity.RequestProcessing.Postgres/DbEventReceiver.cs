using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Curiosity.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace Curiosity.RequestProcessing.Postgres
{
    /// <summary>
    /// Подписчик на события БД.
    /// </summary>
    /// <remarks>
    /// Создаёт новое подключение к БД и бесконечно слушает события от него.
    /// При получении события от БД уведомляет своих подписчиков об этом.
    /// В случае любых ошибок пытается переподключиться к БД.
    /// </remarks>
    public class DbEventReceiver : BackgroundService, IEventReceiver
    {
        private readonly PostgresEventReceiverOptions _eventReceiverOptions;
        private readonly MonitoredDatabase _dbOptions;
        private readonly ILogger _logger;

        /// <summary>
        /// Список событий, на который подписан этот экземпляр.
        /// </summary>
        private readonly HashSet<string> _eventsToSubscribe;

        /// <summary>
        /// Получение события от Postgres, на которое мы подписаны.
        /// </summary>
        public event EventHandler<IRequestProcessingEvent>? OnEventReceived;

        private string? _databaseName;
        private string? _databaseHost;

        public DbEventReceiver(
            PostgresEventReceiverOptions eventReceiverOptions,
            MonitoredDatabase monitoredDatabase,
            ILogger<DbEventReceiver> logger)
        {
            _eventReceiverOptions = eventReceiverOptions ?? throw new ArgumentNullException(nameof(eventReceiverOptions));
            eventReceiverOptions.AssertValid();
            _dbOptions = monitoredDatabase ?? throw new ArgumentNullException(nameof(monitoredDatabase));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _eventsToSubscribe = new HashSet<string>(eventReceiverOptions.EventNames);
        }
        
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Yield();
            
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var strBuilder = new NpgsqlConnectionStringBuilder(_dbOptions.ConnectionString)
                    {
                        KeepAlive = _eventReceiverOptions.KeepAliveSec
                    };

                    await using (var connection = new NpgsqlConnection(strBuilder.ConnectionString))
                    {
                        _databaseName = connection.Database;
                        _databaseHost = connection.Host;
                        _logger.LogDebug($"Подключаемся к БД {GetDbLogName()} для подписки на события PG...");

                        await connection.OpenAsync(stoppingToken);
                        connection.Notification += HandleDbNotification;

                        await using (var command = BuildSubscribeCommand(connection))
                        {
                            await command.ExecuteNonQueryAsync(stoppingToken);
                        }

                        _logger.LogDebug($"Соединение с БД {GetDbLogName()} установлено. Подписка на события выполнена. Ожидаем события...");
                        while (!stoppingToken.IsCancellationRequested)
                        {
                            _logger.LogTrace($"Ожидаем событий от БД {GetDbLogName()}...");
                            await connection.WaitAsync(stoppingToken);
                            _logger.LogTrace($"Получили события от БД {GetDbLogName()}");
                        }

                        _logger.LogDebug($"Отписываемся от событий БД {GetDbLogName()}...");
                        connection.Notification -= HandleDbNotification;
                    }
                }
                catch (Exception) when (stoppingToken.IsCancellationRequested)
                {
                    // Идет остановка, все под контролем
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Возникла ошибка при поддержании или установлении подключения к БД {GetDbLogName()} для обработки событий. Причина: {ex.Message}");

                    _logger.LogInformation($"Восстановление соединение с БД {GetDbLogName()} произойдет через {_eventReceiverOptions.ReconnectionPauseMs} мс...");
                    await Task.Delay(TimeSpan.FromMilliseconds(_eventReceiverOptions.ReconnectionPauseMs), stoppingToken);
                }
            }
            
            _logger.LogDebug($"Обработка событий от БД {GetDbLogName()} остановлена");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private string GetDbLogName()
        {
            return $"{_databaseName} \"({_databaseHost})\"";
        }

        /// <summary>
        /// Создаёт команду PostgreSQL для подписки на нужные события.
        /// </summary>
        private NpgsqlCommand BuildSubscribeCommand(NpgsqlConnection connection)
        {
            var strBuilder = new StringBuilder();
            foreach (var eventName in _eventsToSubscribe)
            {
                if (String.IsNullOrWhiteSpace(eventName))
                {
                    _logger.LogWarning("В списке события для подписки есть пустая строка");
                    continue;
                }
                
                strBuilder.AppendLine($"listen {eventName};");
            }

            var commandQuery = strBuilder.ToString();
            return new NpgsqlCommand(commandQuery, connection);
        }

        /// <summary>
        /// Вызывает нужный метод-обработчик на основе типа события из БД
        /// </summary>
        private void HandleDbNotification(object sender, NpgsqlNotificationEventArgs? e)
        {
            _logger.LogTrace($"Получено новое событие от БД {GetDbLogName()}: " +
                             $"PID = {e?.PID}, " +
                             $"Channel = \"{e?.Channel}\", " +
                             $"Payload = \"{e?.Payload}\"");

            if (!String.IsNullOrEmpty(e?.Channel) && _eventsToSubscribe.Contains(e.Channel))
            {
                try
                {
                    _logger.LogTrace($"Вызов обработчика для события от БД {GetDbLogName()} с PID = {e.PID} (channel = \"{e.Channel}\")...");
                    OnEventReceived?.Invoke(this, new DbEvent(_dbOptions, e.Channel, e.Payload));
                    _logger.LogTrace($"Вызов обработчика для события от БД {GetDbLogName()} с PID = {e.PID} (channel = \"{e.Channel}\") завершен");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Необработанное исключение при вызове обработка события от БД {GetDbLogName()} с PID = {e.PID} (channel = \"{e.Channel}\")");
                }
            }
            else
            {
                _logger.LogTrace($"Нет обработчиков для события из БД {GetDbLogName()} c PID = {e?.PID} (channel = \"{e?.Channel}\")");
            }
        }
    }
}
