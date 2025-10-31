using System.Collections.Concurrent;
using HR.Application.Abstractions.Repositories;
using HR.Domain.Entities;

namespace HR.Infrastructure.Persistence.Repositories;

public sealed class InMemoryCommsAnnouncementRepository : ICommsAnnouncementRepository
{
    private readonly ConcurrentDictionary<Guid, CommsAnnouncement> _store = new();

    public Task<CommsAnnouncement> AddAsync(CommsAnnouncement entity, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);
        if (!_store.TryAdd(entity.Id, entity))
        {
            throw new InvalidOperationException($"Announcement with id '{entity.Id}' already exists.");
        }
        return Task.FromResult(entity);
    }

    public Task<CommsAnnouncement?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _store.TryGetValue(id, out var value);
        return Task.FromResult(value);
    }

    public Task<CommsAnnouncement?> UpdateAsync(CommsAnnouncement entity, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);
        if (!_store.ContainsKey(entity.Id))
        {
            return Task.FromResult<CommsAnnouncement?>(null);
        }
        _store[entity.Id] = entity;
        return Task.FromResult<CommsAnnouncement?>(entity);
    }

    public Task<IReadOnlyCollection<CommsAnnouncement>> GetByOrganizationAsync(Guid organizationId, CancellationToken cancellationToken = default)
    {
        var result = _store.Values.Where(a => a.OrganizationId == organizationId).ToArray();
        return Task.FromResult<IReadOnlyCollection<CommsAnnouncement>>(result);
    }
}

