using System.Collections.Concurrent;
using HR.Application.Abstractions.Repositories;
using HR.Domain.Entities;

namespace HR.Infrastructure.Persistence.Repositories;

/// <summary>
///     In-memory repository implementation for delegated authority records.
/// </summary>
public sealed class InMemoryDelegatedAuthorityRepository : IDelegatedAuthorityRepository
{
    private readonly ConcurrentDictionary<Guid, DelegatedAuthority> _authorities = new();

    public Task<IReadOnlyCollection<DelegatedAuthority>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        IReadOnlyCollection<DelegatedAuthority> snapshot = _authorities.Values.ToList();
        return Task.FromResult(snapshot);
    }

    public Task<DelegatedAuthority?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _authorities.TryGetValue(id, out var authority);
        return Task.FromResult(authority);
    }

    public Task<IReadOnlyCollection<DelegatedAuthority>> GetByGrantorAsync(
        Guid employeeId,
        CancellationToken cancellationToken = default)
    {
        var results = _authorities.Values
            .Where(authority => authority.GrantorEmployeeId == employeeId)
            .ToList();

        return Task.FromResult<IReadOnlyCollection<DelegatedAuthority>>(results);
    }

    public Task<IReadOnlyCollection<DelegatedAuthority>> GetByDelegateAsync(
        Guid employeeId,
        CancellationToken cancellationToken = default)
    {
        var results = _authorities.Values
            .Where(authority => authority.DelegateEmployeeId == employeeId)
            .ToList();

        return Task.FromResult<IReadOnlyCollection<DelegatedAuthority>>(results);
    }

    public Task<DelegatedAuthority> AddAsync(DelegatedAuthority authority, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(authority);

        if (!_authorities.TryAdd(authority.Id, authority))
        {
            throw new InvalidOperationException($"A delegated authority with id '{authority.Id}' already exists.");
        }

        return Task.FromResult(authority);
    }

    public Task<DelegatedAuthority?> UpdateAsync(DelegatedAuthority authority, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(authority);

        if (!_authorities.ContainsKey(authority.Id))
        {
            return Task.FromResult<DelegatedAuthority?>(null);
        }

        _authorities[authority.Id] = authority;
        return Task.FromResult<DelegatedAuthority?>(authority);
    }

    public Task<bool> RemoveAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_authorities.TryRemove(id, out _));
    }
}
