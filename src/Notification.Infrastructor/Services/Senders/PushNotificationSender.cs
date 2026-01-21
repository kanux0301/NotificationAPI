using Microsoft.Extensions.Logging;
using Notification.Domain.Entities;
using Notification.Domain.Enums;
using Notification.Domain.Services;

namespace Notification.Infrastructure.Services.Senders;

public class PushNotificationSender : INotificationSender
{
    private readonly ILogger<PushNotificationSender> _logger;

    public ChannelType Channel => ChannelType.Push;

    public PushNotificationSender(ILogger<PushNotificationSender> logger)
    {
        _logger = logger;
    }

    public async Task<NotificationSendResult> SendAsync(NotificationMessage notification, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation(
                "Sending push notification {NotificationId} to device {DeviceToken}",
                notification.Id,
                notification.Recipient.Address);

            // TODO: Integrate with actual push notification service (Firebase FCM, Apple APNs, etc.)
            // For now, simulate sending
            await Task.Delay(100, cancellationToken);

            var externalId = Guid.NewGuid().ToString();

            _logger.LogInformation(
                "Push notification {NotificationId} sent successfully with external ID {ExternalId}",
                notification.Id,
                externalId);

            return NotificationSendResult.Success(externalId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Failed to send push notification {NotificationId}",
                notification.Id);

            return NotificationSendResult.Failure(ex.Message);
        }
    }
}
