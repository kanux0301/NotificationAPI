using Notification.Domain.Entities;
using Notification.Domain.Enums;

namespace Notification.Domain.Services;

public interface INotificationSender
{
    ChannelType Channel { get; }
    Task<NotificationSendResult> SendAsync(NotificationMessage notification, CancellationToken cancellationToken = default);
}

public record NotificationSendResult
{
    public bool IsSuccess { get; init; }
    public string? ExternalId { get; init; }
    public string? ErrorMessage { get; init; }
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;

    public static NotificationSendResult Success(string? externalId = null)
        => new() { IsSuccess = true, ExternalId = externalId };

    public static NotificationSendResult Failure(string errorMessage)
        => new() { IsSuccess = false, ErrorMessage = errorMessage };
}
