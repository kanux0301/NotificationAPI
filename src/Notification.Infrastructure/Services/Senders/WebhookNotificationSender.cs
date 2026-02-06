using Microsoft.Extensions.Logging;
using Notification.Domain.Entities;
using Notification.Domain.Enums;
using Notification.Domain.Services;
using System.Text;
using System.Text.Json;

namespace Notification.Infrastructure.Services.Senders;

public class WebhookNotificationSender : INotificationSender
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<WebhookNotificationSender> _logger;

    public ChannelType Channel => ChannelType.Webhook;

    public WebhookNotificationSender(
        IHttpClientFactory httpClientFactory,
        ILogger<WebhookNotificationSender> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public async Task<NotificationSendResult> SendAsync(NotificationMessage notification, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation(
                "Sending webhook notification {NotificationId} to {WebhookUrl}",
                notification.Id,
                notification.Recipient.Address);

            var client = _httpClientFactory.CreateClient("WebhookClient");

            var payload = new
            {
                notificationId = notification.Id,
                subject = notification.Content.Subject,
                body = notification.Content.Body,
                timestamp = DateTime.UtcNow,
                metadata = notification.Metadata
            };

            var content = new StringContent(
                JsonSerializer.Serialize(payload),
                Encoding.UTF8,
                "application/json");

            var response = await client.PostAsync(notification.Recipient.Address, content, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation(
                    "Webhook notification {NotificationId} sent successfully",
                    notification.Id);

                return NotificationSendResult.Success(response.Headers.Location?.ToString());
            }

            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogWarning(
                "Webhook notification {NotificationId} failed with status {StatusCode}: {Error}",
                notification.Id,
                response.StatusCode,
                errorContent);

            return NotificationSendResult.Failure($"HTTP {(int)response.StatusCode}: {errorContent}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Failed to send webhook notification {NotificationId}",
                notification.Id);

            return NotificationSendResult.Failure(ex.Message);
        }
    }
}
