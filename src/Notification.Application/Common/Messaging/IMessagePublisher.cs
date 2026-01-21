namespace Notification.Application.Common.Messaging;

/// <summary>
/// Abstraction for publishing messages to a message queue.
/// Implementations: RabbitMQ, Azure Service Bus, Kafka, etc.
/// </summary>
public interface IMessagePublisher
{
    /// <summary>
    /// Publish a message to the specified queue/topic.
    /// </summary>
    Task PublishAsync<T>(T message, string queueName, CancellationToken cancellationToken = default) where T : class;

    /// <summary>
    /// Publish a message with a delay (for scheduled notifications).
    /// </summary>
    Task PublishWithDelayAsync<T>(T message, string queueName, TimeSpan delay, CancellationToken cancellationToken = default) where T : class;

    /// <summary>
    /// Publish multiple messages in a batch.
    /// </summary>
    Task PublishBatchAsync<T>(IEnumerable<T> messages, string queueName, CancellationToken cancellationToken = default) where T : class;
}
