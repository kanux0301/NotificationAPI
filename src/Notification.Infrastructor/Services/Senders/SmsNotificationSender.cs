using Microsoft.Extensions.Logging;
using Notification.Domain.Entities;
using Notification.Domain.Enums;
using Notification.Domain.Services;

namespace Notification.Infrastructure.Services.Senders;

public class SmsNotificationSender : INotificationSender
{
    private readonly ILogger<SmsNotificationSender> _logger;

    public ChannelType Channel => ChannelType.Sms;

    public SmsNotificationSender(ILogger<SmsNotificationSender> logger)
    {
        _logger = logger;
    }

    public async Task<NotificationSendResult> SendAsync(NotificationMessage notification, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation(
                "Sending SMS notification {NotificationId} to {Recipient}",
                notification.Id,
                notification.Recipient.Address);

            // TODO: Integrate with actual SMS service (Twilio, AWS SNS, etc.)
            // For now, simulate sending
            await Task.Delay(100, cancellationToken);

            var externalId = Guid.NewGuid().ToString();

            _logger.LogInformation(
                "SMS notification {NotificationId} sent successfully with external ID {ExternalId}",
                notification.Id,
                externalId);

            return NotificationSendResult.Success(externalId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Failed to send SMS notification {NotificationId}",
                notification.Id);

            return NotificationSendResult.Failure(ex.Message);
        }
    }
}
