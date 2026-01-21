namespace Notification.Domain.Repositories;

public interface IUnitOfWork : IDisposable
{
    INotificationRepository Notifications { get; }
    INotificationTemplateRepository Templates { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}
