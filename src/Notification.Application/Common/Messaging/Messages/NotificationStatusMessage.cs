using Notification.Domain.Enums;

namespace Notification.Application.Common.Messaging.Messages;

/// <summary>
/// Message sent by channel services back to the orchestrator
/// to report delivery status updates.
/// </summary>
public record NotificationStatusMessage
{
    /// <summary>
    /// The notification ID this status update is for.
    /// </summary>
    public Guid NotificationId { get; init; }

    /// <summary>
    /// Correlation ID for distributed tracing.
    /// </summary>
    public string CorrelationId { get; init; } = null!;

    /// <summary>
    /// The new status of the notification.
    /// </summary>
    public NotificationStatus Status { get; init; }

    /// <summary>
    /// Channel that processed this notification.
    /// </summary>
    public ChannelType Channel { get; init; }

    /// <summary>
    /// External ID from the channel provider (e.g., SendGrid message ID).
    /// </summary>
    public string? ExternalId { get; init; }

    /// <summary>
    /// Error message if the notification failed.
    /// </summary>
    public string? ErrorMessage { get; init; }

    /// <summary>
    /// Error code from the channel provider.
    /// </summary>
    public string? ErrorCode { get; init; }

    /// <summary>
    /// When this status change occurred.
    /// </summary>
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;

    /// <summary>
    /// Current retry count.
    /// </summary>
    public int RetryCount { get; init; }

    /// <summary>
    /// Whether this notification should be retried.
    /// </summary>
    public bool ShouldRetry { get; init; }
}
