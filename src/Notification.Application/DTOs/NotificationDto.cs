using Notification.Domain.Enums;

namespace Notification.Application.DTOs;

public record NotificationDto
{
    public Guid Id { get; init; }
    public string RecipientAddress { get; init; } = null!;
    public string? RecipientName { get; init; }
    public ChannelType Channel { get; init; }
    public string Subject { get; init; } = null!;
    public string Body { get; init; } = null!;
    public NotificationStatus Status { get; init; }
    public NotificationPriority Priority { get; init; }
    public Guid? TemplateId { get; init; }
    public DateTime? ScheduledAt { get; init; }
    public DateTime? SentAt { get; init; }
    public DateTime? DeliveredAt { get; init; }
    public string? FailureReason { get; init; }
    public int RetryCount { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
    public Dictionary<string, string> Metadata { get; init; } = [];
}
