using Notification.Domain.Common;
using Notification.Domain.Enums;

namespace Notification.Domain.Events;

public sealed record NotificationCreatedEvent(Guid NotificationId, ChannelType Channel) : DomainEvent;

public sealed record NotificationSentEvent(Guid NotificationId, ChannelType Channel, DateTime SentAt) : DomainEvent;

public sealed record NotificationDeliveredEvent(Guid NotificationId, DateTime DeliveredAt) : DomainEvent;

public sealed record NotificationFailedEvent(Guid NotificationId, string Reason, int RetryCount) : DomainEvent;

public sealed record NotificationCancelledEvent(Guid NotificationId) : DomainEvent;

public sealed record NotificationRetryScheduledEvent(Guid NotificationId, int RetryCount) : DomainEvent;
