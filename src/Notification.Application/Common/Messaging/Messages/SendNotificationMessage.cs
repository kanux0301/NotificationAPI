using Notification.Domain.Enums;

namespace Notification.Application.Common.Messaging.Messages;

/// <summary>
/// Message contract sent to channel microservices.
/// Each channel service consumes this message and processes the notification.
/// </summary>
public record SendNotificationMessage
{
    /// <summary>
    /// Unique identifier for tracking.
    /// </summary>
    public Guid NotificationId { get; init; }

    /// <summary>
    /// Correlation ID for distributed tracing.
    /// </summary>
    public string CorrelationId { get; init; } = Guid.NewGuid().ToString();

    /// <summary>
    /// The channel this notification should be sent through.
    /// </summary>
    public ChannelType Channel { get; init; }

    /// <summary>
    /// Recipient information.
    /// </summary>
    public RecipientInfo Recipient { get; init; } = null!;

    /// <summary>
    /// Notification content.
    /// </summary>
    public ContentInfo Content { get; init; } = null!;

    /// <summary>
    /// Priority level for processing order.
    /// </summary>
    public NotificationPriority Priority { get; init; }

    /// <summary>
    /// Template ID if using a template.
    /// </summary>
    public Guid? TemplateId { get; init; }

    /// <summary>
    /// Additional metadata for the channel service.
    /// </summary>
    public Dictionary<string, string> Metadata { get; init; } = [];

    /// <summary>
    /// Timestamp when the message was created.
    /// </summary>
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;

    /// <summary>
    /// Number of retry attempts (incremented by channel service on failure).
    /// </summary>
    public int RetryCount { get; init; }
}

public record RecipientInfo
{
    public string Address { get; init; } = null!;
    public string? Name { get; init; }
}

public record ContentInfo
{
    public string Subject { get; init; } = null!;
    public string Body { get; init; } = null!;
    public bool IsHtml { get; init; }
}
