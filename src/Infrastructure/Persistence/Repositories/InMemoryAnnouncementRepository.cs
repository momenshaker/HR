using System.Collections.Concurrent;
using HR.Application.Abstractions.Repositories;
using HR.Domain.Entities;

namespace HR.Infrastructure.Persistence.Repositories;

/// <summary>
///     In-memory repository implementation for internal announcements.
/// </summary>
public sealed class InMemoryAnnouncementRepository : IAnnouncementRepository
{
    private readonly ConcurrentDictionary<Guid, Announcement> _announcements = new();

    public Task<IReadOnlyCollection<Announcement>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        IReadOnlyCollection<Announcement> snapshot = _announcements.Values.ToList();
        return Task.FromResult(snapshot);
    }

    public Task<Announcement?> GetByIdAsync(Guid announcementId, CancellationToken cancellationToken = default)
    {
        _announcements.TryGetValue(announcementId, out var announcement);
        return Task.FromResult(announcement);
    }

    public Task<Announcement> AddAsync(Announcement announcement, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(announcement);

        if (!_announcements.TryAdd(announcement.Id, announcement))
        {
            throw new InvalidOperationException($"An announcement with id '{announcement.Id}' already exists.");
        }

        return Task.FromResult(announcement);
    }

    public Task<Announcement?> UpdateAsync(Announcement announcement, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(announcement);

        if (!_announcements.ContainsKey(announcement.Id))
        {
            return Task.FromResult<Announcement?>(null);
        }

        _announcements[announcement.Id] = announcement;
        return Task.FromResult<Announcement?>(announcement);
    }

    public Task<bool> RemoveAsync(Guid announcementId, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_announcements.TryRemove(announcementId, out _));
    }
}
