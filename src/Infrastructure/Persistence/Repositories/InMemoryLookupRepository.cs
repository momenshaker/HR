using System.Collections.Concurrent;
using HR.Application.Abstractions.Repositories;
using HR.Domain.Entities;

namespace HR.Infrastructure.Persistence.Repositories;

public sealed class InMemoryLookupRepository : ILookupRepository
{
    private readonly ConcurrentDictionary<Guid, LookupValue> _store = new();

    public Task<IReadOnlyCollection<LookupValue>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        IReadOnlyCollection<LookupValue> values = _store.Values.ToList();
        return Task.FromResult(values);
    }

    public Task<IReadOnlyCollection<LookupValue>> GetByCategoryAsync(
        string category,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(category);
        var items = _store.Values
            .Where(value => value.Category == category)
            .OrderBy(value => value.SortOrder)
            .ThenBy(value => value.DisplayName, StringComparer.OrdinalIgnoreCase)
            .ToList();

        return Task.FromResult<IReadOnlyCollection<LookupValue>>(items);
    }

    public Task<LookupValue?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _store.TryGetValue(id, out var value);
        return Task.FromResult(value);
    }

    public Task<LookupValue> AddAsync(LookupValue value, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(value);

        if (!_store.TryAdd(value.Id, value))
        {
            throw new InvalidOperationException($"Lookup value '{value.Id}' already exists.");
        }

        return Task.FromResult(value);
    }

    public Task<LookupValue?> UpdateAsync(LookupValue value, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(value);

        if (!_store.ContainsKey(value.Id))
        {
            return Task.FromResult<LookupValue?>(null);
        }

        _store[value.Id] = value;
        return Task.FromResult<LookupValue?>(value);
    }

    public Task<bool> RemoveAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_store.TryRemove(id, out _));
    }

    public Task<bool> ExistsByCodeAsync(
        string category,
        string code,
        Guid? excludingId,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(category);
        ArgumentException.ThrowIfNullOrWhiteSpace(code);

        var exists = _store.Values.Any(value =>
            value.Category == category &&
            value.Code == code &&
            (!excludingId.HasValue || value.Id != excludingId.Value));

        return Task.FromResult(exists);
    }

    public Task<int> GetNextSortOrderAsync(string category, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(category);

        var max = _store.Values
            .Where(value => value.Category == category)
            .Select(value => value.SortOrder)
            .DefaultIfEmpty(0)
            .Max();

        return Task.FromResult(max + 1);
    }
}
