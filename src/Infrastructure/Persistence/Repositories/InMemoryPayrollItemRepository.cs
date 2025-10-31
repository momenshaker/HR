using System.Collections.Concurrent;
using HR.Application.Abstractions.Repositories;
using HR.Domain.Entities;

namespace HR.Infrastructure.Persistence.Repositories;

public sealed class InMemoryPayrollItemRepository : IPayrollItemRepository
{
    private readonly ConcurrentDictionary<Guid, PayrollItem> _items = new();

    public Task<IReadOnlyCollection<PayrollItem>> GetByRunAsync(Guid runId, CancellationToken cancellationToken = default)
    {
        IReadOnlyCollection<PayrollItem> snapshot = _items.Values.Where(i => i.RunId == runId).ToList();
        return Task.FromResult(snapshot);
    }

    public Task<PayrollItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _items.TryGetValue(id, out var item);
        return Task.FromResult(item);
    }

    public Task AddRangeAsync(IEnumerable<PayrollItem> items, CancellationToken cancellationToken = default)
    {
        foreach (var item in items)
        {
            _items[item.Id] = item;
        }
        return Task.CompletedTask;
    }

    public Task RemoveByRunAsync(Guid runId, CancellationToken cancellationToken = default)
    {
        foreach (var kv in _items.Where(kv => kv.Value.RunId == runId).ToList())
        {
            _items.TryRemove(kv.Key, out _);
        }
        return Task.CompletedTask;
    }
}

