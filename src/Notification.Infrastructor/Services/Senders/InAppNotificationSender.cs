using Microsoft.Extensions.Logging;
using Notification.Domain.Entities;
using Notification.Domain.Enums;
using Notification.Domain.Services;

namespace Notification.Infrastructure.Services.Senders;

public class InAppNotificationSender : INotificationSender
{
    private readonly ILogger<InAppNotificationSender> _logger;

    public ChannelType Channel => ChannelType.InApp;

    public InAppNotificationSender(ILogger<InAppNotificationSender> logger)
    {
        _logger = logger;
    }

    public async Task<NotificationSendResult> SendAsync(NotificationMessage notification, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation(
                "Sending in-app notification {NotificationId} to user {UserId}",
                notification.Id,
                notification.Recipient.Address);

            // TODO: Integrate with actual in-app notification service (SignalR, WebSocket, etc.)
            // For now, simulate storing the notification for the user to fetch
            await Task.Delay(50, cancellationToken);

            _logger.LogInformation(
                "In-app notification {NotificationId} sent successfully",
                notification.Id);

            return NotificationSendResult.Success(notification.Id.ToString());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Failed to send in-app notification {NotificationId}",
                notification.Id);

            return NotificationSendResult.Failure(ex.Message);
        }
    }
}
