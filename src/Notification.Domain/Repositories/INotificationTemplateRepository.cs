using Notification.Domain.Entities;
using Notification.Domain.Enums;

namespace Notification.Domain.Repositories;

public interface INotificationTemplateRepository : IRepository<NotificationTemplate, Guid>
{
    Task<NotificationTemplate?> GetByNameAsync(
        string name,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<NotificationTemplate>> GetByChannelAsync(
        ChannelType channel,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<NotificationTemplate>> GetActiveTemplatesAsync(
        CancellationToken cancellationToken = default);

    Task<bool> ExistsByNameAsync(
        string name,
        CancellationToken cancellationToken = default);
}
