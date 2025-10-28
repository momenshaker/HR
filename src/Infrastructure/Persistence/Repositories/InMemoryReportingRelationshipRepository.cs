using System.Collections.Concurrent;
using HR.Application.Abstractions.Repositories;
using HR.Domain.Entities;

namespace HR.Infrastructure.Persistence.Repositories;

/// <summary>
///     In-memory repository implementation for reporting relationships.
/// </summary>
public sealed class InMemoryReportingRelationshipRepository : IReportingRelationshipRepository
{
    private readonly ConcurrentDictionary<Guid, ReportingRelationship> _relationships = new();

    public Task<IReadOnlyCollection<ReportingRelationship>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        IReadOnlyCollection<ReportingRelationship> snapshot = _relationships.Values.ToList();
        return Task.FromResult(snapshot);
    }

    public Task<ReportingRelationship?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _relationships.TryGetValue(id, out var relationship);
        return Task.FromResult(relationship);
    }

    public Task<IReadOnlyCollection<ReportingRelationship>> GetByManagerPositionAsync(
        Guid managerPositionId,
        CancellationToken cancellationToken = default)
    {
        var results = _relationships.Values
            .Where(relationship => relationship.ManagerPositionId == managerPositionId)
            .ToList();

        return Task.FromResult<IReadOnlyCollection<ReportingRelationship>>(results);
    }

    public Task<IReadOnlyCollection<ReportingRelationship>> GetByReportPositionAsync(
        Guid reportPositionId,
        CancellationToken cancellationToken = default)
    {
        var results = _relationships.Values
            .Where(relationship => relationship.ReportPositionId == reportPositionId)
            .ToList();

        return Task.FromResult<IReadOnlyCollection<ReportingRelationship>>(results);
    }

    public Task<ReportingRelationship> AddAsync(ReportingRelationship relationship, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(relationship);

        if (!_relationships.TryAdd(relationship.Id, relationship))
        {
            throw new InvalidOperationException($"A reporting relationship with id '{relationship.Id}' already exists.");
        }

        return Task.FromResult(relationship);
    }

    public Task<ReportingRelationship?> UpdateAsync(ReportingRelationship relationship, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(relationship);

        if (!_relationships.ContainsKey(relationship.Id))
        {
            return Task.FromResult<ReportingRelationship?>(null);
        }

        _relationships[relationship.Id] = relationship;
        return Task.FromResult<ReportingRelationship?>(relationship);
    }

    public Task<bool> RemoveAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_relationships.TryRemove(id, out _));
    }
}
