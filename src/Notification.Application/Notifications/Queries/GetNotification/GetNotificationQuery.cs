using Notification.Application.Common;
using Notification.Application.DTOs;

namespace Notification.Application.Notifications.Queries.GetNotification;

public record GetNotificationQuery(Guid NotificationId) : IQuery<NotificationDto>;
