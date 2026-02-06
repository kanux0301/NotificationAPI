using Microsoft.EntityFrameworkCore.Storage;
using Notification.Domain.Repositories;
using Notification.Infrastructure.Persistence.Repositories;

namespace Notification.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly NotificationDbContext _context;
    private IDbContextTransaction? _transaction;

    private INotificationRepository? _notifications;
    private INotificationTemplateRepository? _templates;

    public UnitOfWork(NotificationDbContext context)
    {
        _context = context;
    }

    public INotificationRepository Notifications =>
        _notifications ??= new NotificationRepository(_context);

    public INotificationTemplateRepository Templates =>
        _templates ??= new NotificationTemplateRepository(_context);

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction is not null)
        {
            await _transaction.CommitAsync(cancellationToken);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction is not null)
        {
            await _transaction.RollbackAsync(cancellationToken);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
        GC.SuppressFinalize(this);
    }
}
