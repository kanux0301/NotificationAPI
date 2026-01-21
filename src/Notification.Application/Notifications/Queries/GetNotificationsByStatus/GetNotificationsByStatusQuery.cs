using Notification.Application.Common;
using Notification.Application.DTOs;
using Notification.Domain.Enums;

namespace Notification.Application.Notifications.Queries.GetNotificationsByStatus;

public record GetNotificationsByStatusQuery(NotificationStatus Status) : IQuery<IReadOnlyList<NotificationDto>>;
