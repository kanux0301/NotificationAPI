using Notification.Application.Common;
using Notification.Application.DTOs;

namespace Notification.Application.Notifications.Queries.GetNotificationStats;

public record GetNotificationStatsQuery : IQuery<NotificationStatsDto>;
