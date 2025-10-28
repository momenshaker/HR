using System.Collections.Concurrent;
using HR.Application.Abstractions.Repositories;
using HR.Domain.Entities;

namespace HR.Infrastructure.Persistence.Repositories;

/// <summary>
///     In-memory repository implementation for self-service accounts.
/// </summary>
public sealed class InMemorySelfServiceAccountRepository : ISelfServiceAccountRepository
{
    private readonly ConcurrentDictionary<Guid, SelfServiceAccount> _accounts = new();

    public Task<IReadOnlyCollection<SelfServiceAccount>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        IReadOnlyCollection<SelfServiceAccount> snapshot = _accounts.Values.ToList();
        return Task.FromResult(snapshot);
    }

    public Task<SelfServiceAccount?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _accounts.TryGetValue(id, out var account);
        return Task.FromResult(account);
    }

    public Task<SelfServiceAccount?> GetByEmployeeIdAsync(Guid employeeId, CancellationToken cancellationToken = default)
    {
        var account = _accounts.Values.FirstOrDefault(value => value.EmployeeId == employeeId);
        return Task.FromResult(account);
    }

    public Task<SelfServiceAccount> AddAsync(SelfServiceAccount account, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(account);

        if (_accounts.Values.Any(existing => existing.EmployeeId == account.EmployeeId))
        {
            throw new InvalidOperationException("A self-service account already exists for the specified employee.");
        }

        if (!_accounts.TryAdd(account.Id, account))
        {
            throw new InvalidOperationException($"A self-service account with id '{account.Id}' already exists.");
        }

        return Task.FromResult(account);
    }

    public Task<SelfServiceAccount?> UpdateAsync(SelfServiceAccount account, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(account);

        if (!_accounts.ContainsKey(account.Id))
        {
            return Task.FromResult<SelfServiceAccount?>(null);
        }

        _accounts[account.Id] = account;
        return Task.FromResult<SelfServiceAccount?>(account);
    }

    public Task<bool> RemoveAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_accounts.TryRemove(id, out _));
    }
}
