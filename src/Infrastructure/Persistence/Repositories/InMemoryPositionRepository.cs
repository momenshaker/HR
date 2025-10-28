using System.Collections.Concurrent;
using HR.Application.Abstractions.Repositories;
using HR.Domain.Entities;

namespace HR.Infrastructure.Persistence.Repositories;

/// <summary>
///     In-memory repository implementation for organisation positions.
/// </summary>
public sealed class InMemoryPositionRepository : IPositionRepository
{
    private readonly ConcurrentDictionary<Guid, Position> _positions = new();

    public Task<IReadOnlyCollection<Position>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        IReadOnlyCollection<Position> snapshot = _positions.Values.ToList();
        return Task.FromResult(snapshot);
    }

    public Task<Position?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _positions.TryGetValue(id, out var position);
        return Task.FromResult(position);
    }

    public Task<IReadOnlyCollection<Position>> GetByOrganizationUnitAsync(
        Guid organizationUnitId,
        CancellationToken cancellationToken = default)
    {
        var results = _positions.Values
            .Where(position => position.OrganizationUnitId == organizationUnitId)
            .ToList();

        return Task.FromResult<IReadOnlyCollection<Position>>(results);
    }

    public Task<Position?> GetByEmployeeIdAsync(Guid employeeId, CancellationToken cancellationToken = default)
    {
        var position = _positions.Values.FirstOrDefault(p => p.OccupiedByEmployeeId == employeeId);
        return Task.FromResult(position);
    }

    public Task<Position> AddAsync(Position position, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(position);

        if (!_positions.TryAdd(position.Id, position))
        {
            throw new InvalidOperationException($"A position with id '{position.Id}' already exists.");
        }

        return Task.FromResult(position);
    }

    public Task<Position?> UpdateAsync(Position position, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(position);

        if (!_positions.ContainsKey(position.Id))
        {
            return Task.FromResult<Position?>(null);
        }

        _positions[position.Id] = position;
        return Task.FromResult<Position?>(position);
    }

    public Task<bool> RemoveAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_positions.TryRemove(id, out _));
    }
}
