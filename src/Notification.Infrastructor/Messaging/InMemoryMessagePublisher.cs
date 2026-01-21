using Microsoft.Extensions.Logging;
using Notification.Application.Common.Messaging;

namespace Notification.Infrastructure.Messaging;

/// <summary>
/// In-memory message publisher for development and testing.
/// Messages are logged but not actually queued.
/// </summary>
public class InMemoryMessagePublisher : IMessagePublisher
{
    private readonly ILogger<InMemoryMessagePublisher> _logger;

    public InMemoryMessagePublisher(ILogger<InMemoryMessagePublisher> logger)
    {
        _logger = logger;
    }

    public Task PublishAsync<T>(T message, string queueName, CancellationToken cancellationToken = default) where T : class
    {
        _logger.LogInformation(
            "[InMemory] Published message to queue {QueueName}: {@Message}",
            queueName,
            message);

        return Task.CompletedTask;
    }

    public Task PublishWithDelayAsync<T>(T message, string queueName, TimeSpan delay, CancellationToken cancellationToken = default) where T : class
    {
        _logger.LogInformation(
            "[InMemory] Published delayed message to queue {QueueName} with delay {Delay}: {@Message}",
            queueName,
            delay,
            message);

        return Task.CompletedTask;
    }

    public Task PublishBatchAsync<T>(IEnumerable<T> messages, string queueName, CancellationToken cancellationToken = default) where T : class
    {
        var messageList = messages.ToList();

        _logger.LogInformation(
            "[InMemory] Published batch of {Count} messages to queue {QueueName}",
            messageList.Count,
            queueName);

        return Task.CompletedTask;
    }
}
