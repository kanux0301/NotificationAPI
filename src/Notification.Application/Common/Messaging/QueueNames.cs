namespace Notification.Application.Common.Messaging;

/// <summary>
/// Centralized queue/topic names for notification channels.
/// Each channel microservice listens to its respective queue.
/// </summary>
public static class QueueNames
{
    public const string EmailNotifications = "notifications.email";
    public const string SmsNotifications = "notifications.sms";
    public const string PushNotifications = "notifications.push";
    public const string WebhookNotifications = "notifications.webhook";
    public const string InAppNotifications = "notifications.inapp";

    /// <summary>
    /// Status updates from channel services back to the orchestrator.
    /// </summary>
    public const string NotificationStatusUpdates = "notifications.status";

    /// <summary>
    /// Dead letter queue for failed messages.
    /// </summary>
    public const string DeadLetterQueue = "notifications.deadletter";
}
