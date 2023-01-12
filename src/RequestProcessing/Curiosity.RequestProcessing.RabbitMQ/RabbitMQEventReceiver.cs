using System;
using System.Threading;
using System.Threading.Tasks;
using Curiosity.Configuration;
using Curiosity.RequestProcessing.RabbitMQ.Options;
using Curiosity.Tools;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;

namespace Curiosity.RequestProcessing.RabbitMQ;

/// <summary>
/// Service for receiving events from RabbitMQ event source.
/// </summary>
public class RabbitMQEventReceiver : IEventReceiver
{
    /// <summary>
    /// Count of retries to restore connection to RabbitMQ manually if auto recovery fails.
    /// </summary>
    /// <remarks>
    /// Useful when we got an <see cref="AlreadyClosedException"/> or another RabbitMQ exception. 
    /// </remarks>
    private const int MaxConnectionRestoreRetries = 10;

    /// <summary>
    /// Timeout of closing <see cref="_connection"/>.
    /// </summary>
    private static readonly TimeSpan ConnectionCloseTimeout = TimeSpan.FromSeconds(3);

    /// <summary>
    /// Default period for RabbitMQ network recovery.
    /// </summary>
    private static readonly TimeSpan NetworkRecoveryPeriod = TimeSpan.FromSeconds(10);

    private readonly RabbitMQEventReceiverOptions _eventReceiverOptions;
    private readonly ILogger _logger;

    private readonly int _qos;
    private readonly object _lockObject = new();

    /// <summary>
    /// Time for waiting connection full recovering before sending data to the queues.
    /// </summary>
    private readonly TimeSpan _recoverWaitDelay;

    private IConnection? _connection;
    private IModel? _channel;
    private CancellationTokenSource _cts = null!;

    /// <inheritdoc />
    public event EventHandler<IRequestProcessingEvent>? OnEventReceived;

    /// <inheritdoc cref="RabbitMQEventReceiver"/>
    public RabbitMQEventReceiver(
        RabbitMQEventReceiverOptions eventReceiverOptions,
        ILogger logger,
        int qos)
    {
        if (qos < 0) throw new ArgumentOutOfRangeException(nameof(qos));

        _eventReceiverOptions = eventReceiverOptions ?? throw new ArgumentNullException(nameof(eventReceiverOptions));
        eventReceiverOptions.AssertValid();

        _logger = logger;
        _qos = qos;

        // get more time for waiting because of auto recovery
        // give time to auto recovery and only after that we will try to restore all manually
        _recoverWaitDelay = TimeSpan.FromMilliseconds(NetworkRecoveryPeriod.TotalMilliseconds * 2);
    }

    /// <inheritdoc />
    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogDebug("Starting RabbitMQEventReceiver...");
        
        _cts = new CancellationTokenSource();
        
        _logger.LogTrace("Entering lock for connecting to RabbitMQ...");
        lock (_lockObject)
        {
            _logger.LogTrace("Entered lock for connecting to RabbitMQ");
            Connect();
        }
        _logger.LogTrace("Exited lock for connecting to RabbitMQ");

        _logger.LogDebug("Started RabbitMQEventReceiver...");

        return Task.CompletedTask;
    }

    private void Connect()
    {
        var factory = new ConnectionFactory
        {
            HostName = _eventReceiverOptions.HostName,
            AutomaticRecoveryEnabled = true,
            NetworkRecoveryInterval = NetworkRecoveryPeriod,
            TopologyRecoveryEnabled = true,
            UserName = _eventReceiverOptions.UserName,
            Password = _eventReceiverOptions.Password,
            Port = _eventReceiverOptions.Port, 
            ClientProvidedName = $"{_eventReceiverOptions.ClientName}_event_receiver"
        };

        _connection = factory.CreateConnection();
        _connection.ConnectionShutdown += HandleOnDisconnected;

        InitChannel();

        _logger.LogInformation(
            "Connected to RabbitMQ (host \"{RabbitHostName}\", queue = \"{QueueName}\", QoS = {QoS})",
            _eventReceiverOptions.HostName,
            _eventReceiverOptions.QueueName,
            _qos);
    }

    private void HandleOnDisconnected(object? sender, ShutdownEventArgs e)
    {
        _logger.LogWarning(
            "RabbitMQ connection was shutdown. Cause=\"{Cause}\", Initiator={Initiator}, ReplyCode={ReplyCode}, ReplyText={ReplyText}",
            e.Cause,
            e.Initiator,
            e.ReplyCode,
            e.ReplyText);

        // if we got AMQP consumer timeout exception let's try to restore connection
        if (e.ReplyCode == 406 && e.ReplyText.Contains("timeout") || e.ReplyCode == 541)
        {
            HandleRecoverySafelyAsync(1, _cts.Token).WithExceptionLogger(_logger);
        }
    }

    private void InitChannel()
    {
        _channel = _connection!.CreateModel();
        _channel.BasicQos(0, (ushort)_qos, true);
        _channel.ModelShutdown += HandleChannelShutdown;

        _channel.QueueDeclare(_eventReceiverOptions.QueueName, true, false, false, null);

        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += ProcessReceivedEvent;
        _channel.BasicConsume(_eventReceiverOptions.QueueName, false, consumer);
    }

    private void HandleChannelShutdown(object? sender, ShutdownEventArgs e)
    {
        _logger.LogWarning(
            "RabbitMQ reader channel was shutdown. Cause=\"{Cause}\", Initiator={Initiator}, ReplyCode={ReplyCode}, ReplyText={ReplyText}, ClassId={ClassId}, MethodId={MethodId}",
            e.Cause,
            e.Initiator,
            e.ReplyCode,
            e.ReplyText,
            e.ClassId,
            e.MethodId);

        // if we got AMQP consumer timeout exception let's try to restore connection
        if (e.ReplyCode == 406 && e.ReplyText.Contains("timeout") || e.ReplyCode == 541)
        {
            HandleRecoverySafelyAsync(1, _cts.Token).WithExceptionLogger(_logger);
        }
    }

    /// <summary>
    /// Handle connection recovery without throwing any exception. Re-queues specified request if recovery completes successfully.
    /// </summary>
    private async Task<bool> HandleRecoverySafelyAsync(
        int currentRetriesCount = 1,
        CancellationToken cancellationToken = default)
    {
        // maybe there is no need to restore client?
        if (_channel?.IsOpen ?? false) return true;

        if (currentRetriesCount > MaxConnectionRestoreRetries)
        {
            _logger.LogError(
                "Exceeded all attempts to restore connections to RabbitMQ ({CurrentRetriesCount}/{MaxRetriesCount})",
                currentRetriesCount,
                MaxConnectionRestoreRetries);
            return false;
        }

        _logger.LogInformation(
            "Trying to restore connection to RabbitMQ ({CurrentRetriesCount}/{MaxRetriesCount})...",
            currentRetriesCount,
            MaxConnectionRestoreRetries);
        
        // wait for auto recovering
        try
        {
            _logger.LogDebug("Make a delay for {RecoveryWaitDelay} before recovering", _recoverWaitDelay);
            await Task.Delay(_recoverWaitDelay, cancellationToken);
        }
        catch (Exception)
        {
            // ignored
            if (cancellationToken.IsCancellationRequested) return false;
        }

        // maybe there is no need to restore client?
        if (_channel?.IsOpen ?? false) return true;

        bool isRecovered;
        var lockWasTaken = false;
        try
        {
            _logger.LogDebug("Entering lock for restoring connection...");
            Monitor.Enter(_lockObject, ref lockWasTaken);
            _logger.LogDebug("Entered lock for restoring connection");

            // maybe there is no need to restore client?
            if (_channel?.IsOpen ?? false) return true;

            // check, if any channel is still closed, than auto recovering failed, need to restore them manually
            if (_channel?.IsClosed ?? true)
            {
                _logger.LogWarning(
                    "Failed to recover channel automatically. Restoring channel manually ({CurrentRetriesCount}/{MaxRetriesCount})...",
                    currentRetriesCount,
                    MaxConnectionRestoreRetries);

                try
                {
                    _logger.LogDebug("Disconnecting from RabbitMQ...");
                    Disconnect();
                    Connect();

                    isRecovered = true;
                }
                catch (Exception e)
                {
                    _logger.LogWarning(
                        e,
                        "Failed to restore channel manually ({CurrentRetriesCount}/{MaxRetriesCount})",
                        currentRetriesCount,
                        MaxConnectionRestoreRetries);

                    // in case of error try to recover connection until we can
                    // exit lock before recursive method call 
                    if (lockWasTaken)
                    {
                        Monitor.Exit(_lockObject);
                        lockWasTaken = false;
                        _logger.LogDebug("Exited lock for restoring connection");
                    }

                    isRecovered = await HandleRecoverySafelyAsync(currentRetriesCount + 1, cancellationToken);
                }
            }
            else
            {
                _logger.LogInformation(
                    "Channel was restored automatically ({CurrentRetriesCount}/{MaxRetriesCount})",
                    currentRetriesCount,
                    MaxConnectionRestoreRetries);
                isRecovered = true;
            }
        }
        finally
        {
            if (lockWasTaken)
            {
                Monitor.Exit(_lockObject);
                _logger.LogDebug("Exited lock for restoring connection");
            }
        }

        if (currentRetriesCount == 1)
        {
            if (isRecovered)
            {
                _logger.LogInformation(
                    "Successfully recovered connection ({CurrentRetriesCount}/{MaxRetriesCount})",
                    currentRetriesCount,
                    MaxConnectionRestoreRetries);
            }
            else
            {
                _logger.LogError(
                    "Failed to recover connection ({CurrentRetriesCount}/{MaxRetriesCount})",
                    currentRetriesCount,
                    MaxConnectionRestoreRetries);
            }
        }

        return isRecovered;
    }

    /// <summary>
    /// Closes connection to RabbitMQ. Should be invoked only from a critical section.
    /// </summary>
    private void Disconnect()
    {
        // free used channels

        if (_channel != null)
        {
            _channel.ModelShutdown -= HandleChannelShutdown;

            if (!_channel.IsClosed)
            {
                _logger.LogTrace("Waiting for channel closing...");
                try
                {
                    _channel.Close();
                }
                catch (Exception e)
                {
                    _logger.LogWarning(e, "Failed to close channel on RabbitMQ event receiving restoring");
                }
            }
            else
            {
                _logger.LogTrace("Channel have been already closed");
            }

            _logger.LogTrace("Waiting for channel disposing...");
            _channel.Dispose();
        }
        else
        {
            _logger.LogTrace("Reader channel has been already null");
        }
        _channel = null;

        // free connection

        if (_connection != null)
        {
            _connection.ConnectionShutdown -= HandleOnDisconnected;
            
            _logger.LogTrace("Waiting for connection closing...");
            try
            {
                _connection.Close(ConnectionCloseTimeout);
            }
            catch (Exception e)
            {
                _logger.LogWarning(e, "Failed to close connection on rpc client restoring");
            }

            _logger.LogTrace("Waiting for connection disposing...");
            _connection.Dispose();
            _connection = null;
        }
        else
        {
            _logger.LogTrace("Connection has been already null");
        }
    }

    /// <summary>
    /// Processes receives event from RabbitMQ and notifies subscribers.
    /// </summary>
    private void ProcessReceivedEvent(object? sender, BasicDeliverEventArgs e)
    {
        var receivedEvent = new RabbitMQEvent(e, ConfirmEvent, RejectEvent);
        OnEventReceived?.Invoke(this, receivedEvent);
    }

    private void ConfirmEvent(ulong deliveryTag)
    {
        ProcessEventDecision(deliveryTag, EventDecision.Confirm);
    }

    private void ProcessEventDecision(ulong deliveryTag, EventDecision eventDecision)
    {
        Action action;
        string actionName;
        
        switch (eventDecision)
        {
            case EventDecision.Confirm:
                actionName = "confirm";
                action = () =>
                {
                    _channel?.BasicAck(deliveryTag, false);
                };
                break;
            case EventDecision.Reject:
                actionName = "reject";
                action = () =>
                {
                    _channel?.BasicReject(deliveryTag, true);
                };
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(eventDecision), eventDecision, null);
        }

        try
        {
            _logger.LogTrace("Entering lock to {ActionName} message...", actionName);
            lock (_lockObject)
            {
                _logger.LogTrace("Entered lock to {ActionName} message", actionName);
                action.Invoke();
            }
            _logger.LogTrace("Exited lock to {ActionName} message", actionName);

            _logger.LogDebug(
                "Completed {ActionName} receiving response with DeliveryTag={DeliveryTag}",
                actionName,
                deliveryTag);
        }
        catch (RabbitMQClientException e)
        {
            // if we got this exception, probably we have some some connection issues.
            // We will try to wait auto recovering. If it fails, we will recreate the channel

            _logger.LogWarning(e,
                "Got {ExceptionName} exception while {ActionName} response with DeliveryTag={DeliveryTag}. Waiting for recovering for {RecoverWaitDelay}",
                e.GetType().Name,
                actionName,
                deliveryTag,
                _recoverWaitDelay);

            HandleRecoverySafelyAsync(cancellationToken: _cts.Token).WithExceptionLogger(_logger);
        }
        catch (Exception e)
        {
            _logger.LogError(e,
                "Failed to {ActionName} response with DeliveryTag={DeliveryTag}",
                actionName,
                deliveryTag);
        }
    }

    private void RejectEvent(ulong deliveryTag)
    {
        ProcessEventDecision(deliveryTag, EventDecision.Reject);
    }

    /// <inheritdoc />
    public Task StopAsync(CancellationToken cancellationToken)
    {
        // stop sending requests
        _logger.LogTrace("Cancelling cts to stop sending requests...");
        _cts.Cancel();

        // close channel
        _logger.LogTrace("Entering lock to close channel/connection...");
        lock (_lockObject)
        {
            _logger.LogTrace("Entered lock to close channel/connection");
            Disconnect();
        }
        _logger.LogTrace("Exited lock to close channel/connection");

        _logger.LogTrace("Rpc client is fully disposed");

        return Task.CompletedTask;
    }

    private enum EventDecision
    {
        Confirm,
        Reject
    }
}
