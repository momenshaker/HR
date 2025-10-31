using System.Collections.Concurrent;
using HR.Application.Abstractions.Repositories;

namespace HR.Infrastructure.Persistence.Repositories;

public sealed class InMemoryAnnouncementReadRepository : IAnnouncementReadRepository
{
    private readonly ConcurrentDictionary<Guid, HashSet<Guid>> _readsByEmployee = new();

    public Task MarkReadAsync(HR.Domain.Entities.AnnouncementRead read, CancellationToken cancellationToken = default)
    {
        var set = _readsByEmployee.GetOrAdd(read.EmployeeId, _ => new HashSet<Guid>());
        lock (set)
        {
            set.Add(read.AnnouncementId);
        }
        return Task.CompletedTask;
    }

    public Task<IReadOnlyCollection<Guid>> GetReadAnnouncementIdsAsync(Guid employeeId, CancellationToken cancellationToken = default)
    {
        if (_readsByEmployee.TryGetValue(employeeId, out var set))
        {
            lock (set)
            {
                return Task.FromResult<IReadOnlyCollection<Guid>>(set.ToArray());
            }
        }
        return Task.FromResult<IReadOnlyCollection<Guid>>(Array.Empty<Guid>());
    }
}

