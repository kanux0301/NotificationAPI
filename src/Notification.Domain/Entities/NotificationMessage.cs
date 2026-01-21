using Notification.Domain.Common;
using Notification.Domain.Enums;
using Notification.Domain.Events;
using Notification.Domain.ValueObjects;

namespace Notification.Domain.Entities;

public class NotificationMessage : AggregateRoot<Guid>
{
    public Recipient Recipient { get; private set; } = null!;
    public NotificationContent Content { get; private set; } = null!;
    public NotificationStatus Status { get; private set; }
    public NotificationPriority Priority { get; private set; }
    public Guid? TemplateId { get; private set; }
    public DateTime? ScheduledAt { get; private set; }
    public DateTime? SentAt { get; private set; }
    public DateTime? DeliveredAt { get; private set; }
    public string? FailureReason { get; private set; }
    public int RetryCount { get; private set; }
    public Dictionary<string, string> Metadata { get; private set; } = [];

    private NotificationMessage() { }

    private NotificationMessage(
        Guid id,
        Recipient recipient,
        NotificationContent content,
        NotificationPriority priority,
        Guid? templateId,
        DateTime? scheduledAt) : base(id)
    {
        Recipient = recipient;
        Content = content;
        Status = NotificationStatus.Pending;
        Priority = priority;
        TemplateId = templateId;
        ScheduledAt = scheduledAt;
        RetryCount = 0;
    }

    public static NotificationMessage Create(
        Recipient recipient,
        NotificationContent content,
        NotificationPriority priority = NotificationPriority.Normal,
        Guid? templateId = null,
        DateTime? scheduledAt = null)
    {
        var notification = new NotificationMessage(
            Guid.NewGuid(),
            recipient,
            content,
            priority,
            templateId,
            scheduledAt);

        notification.AddDomainEvent(new NotificationCreatedEvent(notification.Id, recipient.Channel));

        return notification;
    }

    public void MarkAsProcessing()
    {
        if (Status != NotificationStatus.Pending)
            throw new InvalidOperationException($"Cannot process notification with status {Status}.");

        Status = NotificationStatus.Processing;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkAsSent()
    {
        if (Status != NotificationStatus.Processing)
            throw new InvalidOperationException($"Cannot mark notification as sent with status {Status}.");

        Status = NotificationStatus.Sent;
        SentAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new NotificationSentEvent(Id, Recipient.Channel, SentAt.Value));
    }

    public void MarkAsDelivered()
    {
        if (Status != NotificationStatus.Sent)
            throw new InvalidOperationException($"Cannot mark notification as delivered with status {Status}.");

        Status = NotificationStatus.Delivered;
        DeliveredAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new NotificationDeliveredEvent(Id, DeliveredAt.Value));
    }

    public void MarkAsFailed(string reason)
    {
        if (Status == NotificationStatus.Delivered || Status == NotificationStatus.Cancelled)
            throw new InvalidOperationException($"Cannot mark notification as failed with status {Status}.");

        Status = NotificationStatus.Failed;
        FailureReason = reason;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new NotificationFailedEvent(Id, reason, RetryCount));
    }

    public void Cancel()
    {
        if (Status != NotificationStatus.Pending && Status != NotificationStatus.Processing)
            throw new InvalidOperationException($"Cannot cancel notification with status {Status}.");

        Status = NotificationStatus.Cancelled;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new NotificationCancelledEvent(Id));
    }

    public bool CanRetry(int maxRetries = 3)
    {
        return Status == NotificationStatus.Failed && RetryCount < maxRetries;
    }

    public void Retry()
    {
        if (Status != NotificationStatus.Failed)
            throw new InvalidOperationException($"Cannot retry notification with status {Status}.");

        Status = NotificationStatus.Pending;
        FailureReason = null;
        RetryCount++;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new NotificationRetryScheduledEvent(Id, RetryCount));
    }

    public void AddMetadata(string key, string value)
    {
        Metadata[key] = value;
        UpdatedAt = DateTime.UtcNow;
    }
}
