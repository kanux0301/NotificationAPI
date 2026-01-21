using Notification.Domain.Entities;
using Notification.Domain.Enums;

namespace Notification.Domain.Repositories;

public interface INotificationRepository : IRepository<NotificationMessage, Guid>
{
    Task<IReadOnlyList<NotificationMessage>> GetByStatusAsync(
        NotificationStatus status,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<NotificationMessage>> GetPendingScheduledAsync(
        DateTime beforeTime,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<NotificationMessage>> GetByRecipientAsync(
        string recipientAddress,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<NotificationMessage>> GetFailedForRetryAsync(
        int maxRetries,
        CancellationToken cancellationToken = default);

    Task<int> GetCountByStatusAsync(
        NotificationStatus status,
        CancellationToken cancellationToken = default);
}
