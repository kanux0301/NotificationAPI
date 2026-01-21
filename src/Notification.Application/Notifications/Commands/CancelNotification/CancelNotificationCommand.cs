using Notification.Application.Common;

namespace Notification.Application.Notifications.Commands.CancelNotification;

public record CancelNotificationCommand(Guid NotificationId) : ICommand;
