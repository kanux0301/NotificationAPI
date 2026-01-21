using Microsoft.EntityFrameworkCore;
using Notification.Domain.Entities;
using Notification.Domain.Enums;
using Notification.Domain.Repositories;

namespace Notification.Infrastructure.Persistence.Repositories;

public class NotificationRepository : INotificationRepository
{
    private readonly NotificationDbContext _context;

    public NotificationRepository(NotificationDbContext context)
    {
        _context = context;
    }

    public async Task<NotificationMessage?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Notifications.FindAsync([id], cancellationToken);
    }

    public async Task<IReadOnlyList<NotificationMessage>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Notifications
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(NotificationMessage entity, CancellationToken cancellationToken = default)
    {
        await _context.Notifications.AddAsync(entity, cancellationToken);
    }

    public Task UpdateAsync(NotificationMessage entity, CancellationToken cancellationToken = default)
    {
        _context.Notifications.Update(entity);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(NotificationMessage entity, CancellationToken cancellationToken = default)
    {
        _context.Notifications.Remove(entity);
        return Task.CompletedTask;
    }

    public async Task<IReadOnlyList<NotificationMessage>> GetByStatusAsync(
        NotificationStatus status,
        CancellationToken cancellationToken = default)
    {
        return await _context.Notifications
            .Where(n => n.Status == status)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<NotificationMessage>> GetPendingScheduledAsync(
        DateTime beforeTime,
        CancellationToken cancellationToken = default)
    {
        return await _context.Notifications
            .Where(n => n.Status == NotificationStatus.Pending
                        && n.ScheduledAt.HasValue
                        && n.ScheduledAt <= beforeTime)
            .OrderBy(n => n.ScheduledAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<NotificationMessage>> GetByRecipientAsync(
        string recipientAddress,
        CancellationToken cancellationToken = default)
    {
        return await _context.Notifications
            .Where(n => n.Recipient.Address == recipientAddress)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<NotificationMessage>> GetFailedForRetryAsync(
        int maxRetries,
        CancellationToken cancellationToken = default)
    {
        return await _context.Notifications
            .Where(n => n.Status == NotificationStatus.Failed && n.RetryCount < maxRetries)
            .OrderBy(n => n.UpdatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> GetCountByStatusAsync(
        NotificationStatus status,
        CancellationToken cancellationToken = default)
    {
        return await _context.Notifications
            .CountAsync(n => n.Status == status, cancellationToken);
    }
}
