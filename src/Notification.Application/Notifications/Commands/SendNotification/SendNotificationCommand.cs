using Notification.Application.Common;
using Notification.Domain.Enums;

namespace Notification.Application.Notifications.Commands.SendNotification;

public record SendNotificationCommand : ICommand<Guid>
{
    public string RecipientAddress { get; init; } = null!;
    public string? RecipientName { get; init; }
    public ChannelType Channel { get; init; }
    public string Subject { get; init; } = null!;
    public string Body { get; init; } = null!;
    public bool IsHtml { get; init; }
    public NotificationPriority Priority { get; init; } = NotificationPriority.Normal;
    public DateTime? ScheduledAt { get; init; }
    public Dictionary<string, string>? Metadata { get; init; }
}
