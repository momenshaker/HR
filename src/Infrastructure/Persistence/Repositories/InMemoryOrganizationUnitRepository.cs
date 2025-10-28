using System.Collections.Concurrent;
using HR.Application.Abstractions.Repositories;
using HR.Domain.Entities;

namespace HR.Infrastructure.Persistence.Repositories;

/// <summary>
///     In-memory repository implementation for organisation units.
/// </summary>
public sealed class InMemoryOrganizationUnitRepository : IOrganizationUnitRepository
{
    private readonly ConcurrentDictionary<Guid, OrganizationUnit> _units = new();

    public Task<IReadOnlyCollection<OrganizationUnit>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        IReadOnlyCollection<OrganizationUnit> snapshot = _units.Values.ToList();
        return Task.FromResult(snapshot);
    }

    public Task<OrganizationUnit?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _units.TryGetValue(id, out var unit);
        return Task.FromResult(unit);
    }

    public Task<OrganizationUnit> AddAsync(OrganizationUnit unit, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(unit);

        if (!_units.TryAdd(unit.Id, unit))
        {
            throw new InvalidOperationException($"An organisation unit with id '{unit.Id}' already exists.");
        }

        return Task.FromResult(unit);
    }

    public Task<OrganizationUnit?> UpdateAsync(OrganizationUnit unit, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(unit);

        if (!_units.ContainsKey(unit.Id))
        {
            return Task.FromResult<OrganizationUnit?>(null);
        }

        _units[unit.Id] = unit;
        return Task.FromResult<OrganizationUnit?>(unit);
    }

    public Task<bool> RemoveAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_units.TryRemove(id, out _));
    }
}
