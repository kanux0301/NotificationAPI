using Microsoft.EntityFrameworkCore;
using Notification.Domain.Entities;
using Notification.Domain.Enums;
using Notification.Domain.Repositories;

namespace Notification.Infrastructure.Persistence.Repositories;

public class NotificationTemplateRepository : INotificationTemplateRepository
{
    private readonly NotificationDbContext _context;

    public NotificationTemplateRepository(NotificationDbContext context)
    {
        _context = context;
    }

    public async Task<NotificationTemplate?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Templates.FindAsync([id], cancellationToken);
    }

    public async Task<IReadOnlyList<NotificationTemplate>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Templates
            .OrderBy(t => t.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(NotificationTemplate entity, CancellationToken cancellationToken = default)
    {
        await _context.Templates.AddAsync(entity, cancellationToken);
    }

    public Task UpdateAsync(NotificationTemplate entity, CancellationToken cancellationToken = default)
    {
        _context.Templates.Update(entity);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(NotificationTemplate entity, CancellationToken cancellationToken = default)
    {
        _context.Templates.Remove(entity);
        return Task.CompletedTask;
    }

    public async Task<NotificationTemplate?> GetByNameAsync(
        string name,
        CancellationToken cancellationToken = default)
    {
        return await _context.Templates
            .FirstOrDefaultAsync(t => t.Name == name, cancellationToken);
    }

    public async Task<IReadOnlyList<NotificationTemplate>> GetByChannelAsync(
        ChannelType channel,
        CancellationToken cancellationToken = default)
    {
        return await _context.Templates
            .Where(t => t.Channel == channel && t.IsActive)
            .OrderBy(t => t.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<NotificationTemplate>> GetActiveTemplatesAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.Templates
            .Where(t => t.IsActive)
            .OrderBy(t => t.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsByNameAsync(
        string name,
        CancellationToken cancellationToken = default)
    {
        return await _context.Templates
            .AnyAsync(t => t.Name == name, cancellationToken);
    }
}
