using Notification.Application.Common;

namespace Notification.Application.Notifications.Commands.RetryNotification;

public record RetryNotificationCommand(Guid NotificationId) : ICommand;
