using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Notification.Application.Common.Messaging;
using RabbitMQ.Client;

namespace Notification.Infrastructure.Messaging;

public class RabbitMqOptions
{
    public const string SectionName = "RabbitMQ";

    public string HostName { get; set; } = "localhost";
    public int Port { get; set; } = 5672;
    public string UserName { get; set; } = "guest";
    public string Password { get; set; } = "guest";
    public string VirtualHost { get; set; } = "/";
    public string ExchangeName { get; set; } = "notifications";
}

public class RabbitMqMessagePublisher : IMessagePublisher, IAsyncDisposable
{
    private readonly ILogger<RabbitMqMessagePublisher> _logger;
    private readonly RabbitMqOptions _options;
    private IConnection? _connection;
    private IChannel? _channel;
    private readonly SemaphoreSlim _connectionLock = new(1, 1);
    private bool _disposed;

    public RabbitMqMessagePublisher(
        IOptions<RabbitMqOptions> options,
        ILogger<RabbitMqMessagePublisher> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    public async Task PublishAsync<T>(T message, string queueName, CancellationToken cancellationToken = default) where T : class
    {
        await EnsureConnectionAsync(cancellationToken);

        var body = SerializeMessage(message);
        var properties = new BasicProperties
        {
            Persistent = true,
            ContentType = "application/json",
            MessageId = Guid.NewGuid().ToString(),
            Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds())
        };

        await _channel!.BasicPublishAsync(
            exchange: _options.ExchangeName,
            routingKey: queueName,
            mandatory: true,
            basicProperties: properties,
            body: body,
            cancellationToken: cancellationToken);

        _logger.LogDebug("Published message to queue {QueueName}", queueName);
    }

    public async Task PublishWithDelayAsync<T>(T message, string queueName, TimeSpan delay, CancellationToken cancellationToken = default) where T : class
    {
        await EnsureConnectionAsync(cancellationToken);

        var body = SerializeMessage(message);
        var delayMs = (int)delay.TotalMilliseconds;

        // Use RabbitMQ delayed message exchange or TTL + dead letter approach
        var properties = new BasicProperties
        {
            Persistent = true,
            ContentType = "application/json",
            MessageId = Guid.NewGuid().ToString(),
            Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds()),
            Headers = new Dictionary<string, object?>
            {
                { "x-delay", delayMs }
            }
        };

        await _channel!.BasicPublishAsync(
            exchange: $"{_options.ExchangeName}.delayed",
            routingKey: queueName,
            mandatory: true,
            basicProperties: properties,
            body: body,
            cancellationToken: cancellationToken);

        _logger.LogDebug("Published delayed message to queue {QueueName} with delay {Delay}ms", queueName, delayMs);
    }

    public async Task PublishBatchAsync<T>(IEnumerable<T> messages, string queueName, CancellationToken cancellationToken = default) where T : class
    {
        await EnsureConnectionAsync(cancellationToken);

        // RabbitMQ.Client 7.x doesn't have batch API, publish individually
        foreach (var message in messages)
        {
            var body = SerializeMessage(message);
            var properties = new BasicProperties
            {
                Persistent = true,
                ContentType = "application/json",
                MessageId = Guid.NewGuid().ToString(),
                Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds())
            };

            await _channel!.BasicPublishAsync(
                exchange: _options.ExchangeName,
                routingKey: queueName,
                mandatory: true,
                basicProperties: properties,
                body: body,
                cancellationToken: cancellationToken);
        }

        _logger.LogDebug("Published batch of messages to queue {QueueName}", queueName);
    }

    private async Task EnsureConnectionAsync(CancellationToken cancellationToken)
    {
        if (_connection is { IsOpen: true } && _channel is { IsOpen: true })
            return;

        await _connectionLock.WaitAsync(cancellationToken);
        try
        {
            if (_connection is { IsOpen: true } && _channel is { IsOpen: true })
                return;

            var factory = new ConnectionFactory
            {
                HostName = _options.HostName,
                Port = _options.Port,
                UserName = _options.UserName,
                Password = _options.Password,
                VirtualHost = _options.VirtualHost
            };

            _connection = await factory.CreateConnectionAsync(cancellationToken);
            _channel = await _connection.CreateChannelAsync(cancellationToken: cancellationToken);

            // Declare exchange
            await _channel.ExchangeDeclareAsync(
                exchange: _options.ExchangeName,
                type: ExchangeType.Direct,
                durable: true,
                autoDelete: false,
                cancellationToken: cancellationToken);

            _logger.LogInformation("RabbitMQ connection established to {Host}:{Port}", _options.HostName, _options.Port);
        }
        finally
        {
            _connectionLock.Release();
        }
    }

    private static ReadOnlyMemory<byte> SerializeMessage<T>(T message)
    {
        var json = JsonSerializer.Serialize(message, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });
        return Encoding.UTF8.GetBytes(json);
    }

    public async ValueTask DisposeAsync()
    {
        if (_disposed) return;

        if (_channel is not null)
        {
            await _channel.CloseAsync();
            _channel.Dispose();
        }

        if (_connection is not null)
        {
            await _connection.CloseAsync();
            _connection.Dispose();
        }

        _connectionLock.Dispose();
        _disposed = true;

        GC.SuppressFinalize(this);
    }
}
