using System.Text.Json;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Notification.Application.Common.Messaging;

namespace Notification.Infrastructure.Messaging;

public class AzureServiceBusOptions
{
    public const string SectionName = "AzureServiceBus";

    public string ConnectionString { get; set; } = null!;
    public string TopicPrefix { get; set; } = "notifications";
}

public class AzureServiceBusMessagePublisher : IMessagePublisher, IAsyncDisposable
{
    private readonly ILogger<AzureServiceBusMessagePublisher> _logger;
    private readonly AzureServiceBusOptions _options;
    private readonly ServiceBusClient _client;
    private readonly Dictionary<string, ServiceBusSender> _senders = [];
    private readonly SemaphoreSlim _senderLock = new(1, 1);
    private bool _disposed;

    public AzureServiceBusMessagePublisher(
        IOptions<AzureServiceBusOptions> options,
        ILogger<AzureServiceBusMessagePublisher> logger)
    {
        _options = options.Value;
        _logger = logger;
        _client = new ServiceBusClient(_options.ConnectionString);
    }

    public async Task PublishAsync<T>(T message, string queueName, CancellationToken cancellationToken = default) where T : class
    {
        var sender = await GetOrCreateSenderAsync(queueName);
        var serviceBusMessage = CreateMessage(message);

        await sender.SendMessageAsync(serviceBusMessage, cancellationToken);

        _logger.LogDebug("Published message to queue {QueueName}", queueName);
    }

    public async Task PublishWithDelayAsync<T>(T message, string queueName, TimeSpan delay, CancellationToken cancellationToken = default) where T : class
    {
        var sender = await GetOrCreateSenderAsync(queueName);
        var serviceBusMessage = CreateMessage(message);

        // Schedule the message for future delivery
        serviceBusMessage.ScheduledEnqueueTime = DateTimeOffset.UtcNow.Add(delay);

        await sender.SendMessageAsync(serviceBusMessage, cancellationToken);

        _logger.LogDebug("Published scheduled message to queue {QueueName} with delay {Delay}", queueName, delay);
    }

    public async Task PublishBatchAsync<T>(IEnumerable<T> messages, string queueName, CancellationToken cancellationToken = default) where T : class
    {
        var sender = await GetOrCreateSenderAsync(queueName);

        // Create a batch
        using var messageBatch = await sender.CreateMessageBatchAsync(cancellationToken);

        foreach (var message in messages)
        {
            var serviceBusMessage = CreateMessage(message);

            if (!messageBatch.TryAddMessage(serviceBusMessage))
            {
                // If batch is full, send current batch and create a new one
                await sender.SendMessagesAsync(messageBatch, cancellationToken);
                using var newBatch = await sender.CreateMessageBatchAsync(cancellationToken);

                if (!newBatch.TryAddMessage(serviceBusMessage))
                {
                    throw new InvalidOperationException("Message too large for batch");
                }
            }
        }

        // Send remaining messages
        if (messageBatch.Count > 0)
        {
            await sender.SendMessagesAsync(messageBatch, cancellationToken);
        }

        _logger.LogDebug("Published batch of messages to queue {QueueName}", queueName);
    }

    private async Task<ServiceBusSender> GetOrCreateSenderAsync(string queueName)
    {
        if (_senders.TryGetValue(queueName, out var existingSender))
            return existingSender;

        await _senderLock.WaitAsync();
        try
        {
            if (_senders.TryGetValue(queueName, out existingSender))
                return existingSender;

            var sender = _client.CreateSender(queueName);
            _senders[queueName] = sender;

            _logger.LogInformation("Created Azure Service Bus sender for queue {QueueName}", queueName);

            return sender;
        }
        finally
        {
            _senderLock.Release();
        }
    }

    private static ServiceBusMessage CreateMessage<T>(T message)
    {
        var json = JsonSerializer.Serialize(message, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        return new ServiceBusMessage(json)
        {
            ContentType = "application/json",
            MessageId = Guid.NewGuid().ToString()
        };
    }

    public async ValueTask DisposeAsync()
    {
        if (_disposed) return;

        foreach (var sender in _senders.Values)
        {
            await sender.DisposeAsync();
        }
        _senders.Clear();

        await _client.DisposeAsync();
        _senderLock.Dispose();
        _disposed = true;

        GC.SuppressFinalize(this);
    }
}
