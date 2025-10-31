using System.Collections.Concurrent;
using HR.Application.Abstractions.Repositories;
using HR.Domain.Entities;

namespace HR.Infrastructure.Persistence.Repositories;

public sealed class InMemoryLeaveTypeRepository : ILeaveTypeRepository
{
    private readonly ConcurrentDictionary<Guid, LeaveType> _store = new();

    public Task<IReadOnlyCollection<LeaveType>> GetAllAsync(CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyCollection<LeaveType>>(_store.Values.ToList());

    public Task<LeaveType?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _store.TryGetValue(id, out var entity);
        return Task.FromResult(entity);
    }

    public Task<LeaveType> AddAsync(LeaveType entity, CancellationToken cancellationToken = default)
    {
        if (!_store.TryAdd(entity.Id, entity))
            throw new InvalidOperationException($"Duplicate key {entity.Id}");
        return Task.FromResult(entity);
    }

    public Task<LeaveType?> UpdateAsync(LeaveType entity, CancellationToken cancellationToken = default)
    {
        if (!_store.ContainsKey(entity.Id)) return Task.FromResult<LeaveType?>(null);
        _store[entity.Id] = entity;
        return Task.FromResult<LeaveType?>(entity);
    }

    public Task<bool> RemoveAsync(Guid id, CancellationToken cancellationToken = default)
        => Task.FromResult(_store.TryRemove(id, out _));
}

