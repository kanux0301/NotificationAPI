using Microsoft.Extensions.Logging;
using Notification.Domain.Entities;
using Notification.Domain.Enums;
using Notification.Domain.Services;

namespace Notification.Infrastructure.Services.Senders;

public class EmailNotificationSender : INotificationSender
{
    private readonly ILogger<EmailNotificationSender> _logger;

    public ChannelType Channel => ChannelType.Email;

    public EmailNotificationSender(ILogger<EmailNotificationSender> logger)
    {
        _logger = logger;
    }

    public async Task<NotificationSendResult> SendAsync(NotificationMessage notification, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation(
                "Sending email notification {NotificationId} to {Recipient}",
                notification.Id,
                notification.Recipient.Address);

            // TODO: Integrate with actual email service (SendGrid, AWS SES, SMTP, etc.)
            // For now, simulate sending
            await Task.Delay(100, cancellationToken);

            var externalId = Guid.NewGuid().ToString();

            _logger.LogInformation(
                "Email notification {NotificationId} sent successfully with external ID {ExternalId}",
                notification.Id,
                externalId);

            return NotificationSendResult.Success(externalId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Failed to send email notification {NotificationId}",
                notification.Id);

            return NotificationSendResult.Failure(ex.Message);
        }
    }
}
