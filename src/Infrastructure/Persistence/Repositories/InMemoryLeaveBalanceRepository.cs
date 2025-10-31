using System.Collections.Concurrent;
using HR.Application.Abstractions.Repositories;
using HR.Domain.Entities;

namespace HR.Infrastructure.Persistence.Repositories;

public sealed class InMemoryLeaveBalanceRepository : ILeaveBalanceRepository
{
    private readonly ConcurrentDictionary<(Guid employeeId, Guid leaveTypeId, int year), LeaveBalance> _store = new();

    public Task<IReadOnlyCollection<LeaveBalance>> GetByEmployeeYearAsync(Guid employeeId, int year, CancellationToken cancellationToken = default)
    {
        var items = _store.Where(kv => kv.Key.employeeId == employeeId && kv.Key.year == year).Select(kv => kv.Value).ToList();
        return Task.FromResult<IReadOnlyCollection<LeaveBalance>>(items);
    }

    public Task<LeaveBalance?> GetAsync(Guid employeeId, Guid leaveTypeId, int year, CancellationToken cancellationToken = default)
    {
        _store.TryGetValue((employeeId, leaveTypeId, year), out var entity);
        return Task.FromResult(entity);
    }

    public Task<LeaveBalance> UpsertAsync(LeaveBalance entity, CancellationToken cancellationToken = default)
    {
        _store[(entity.EmployeeId, entity.LeaveTypeId, entity.Year)] = entity;
        return Task.FromResult(entity);
    }

    public Task<bool> UpdateTakenAsync(Guid employeeId, Guid leaveTypeId, int year, decimal newTaken, byte[]? expectedRowVersion, CancellationToken cancellationToken = default)
    {
        if (_store.TryGetValue((employeeId, leaveTypeId, year), out var existing))
        {
            _store[(employeeId, leaveTypeId, year)] = new LeaveBalance
            {
                EmployeeId = existing.EmployeeId,
                LeaveTypeId = existing.LeaveTypeId,
                Year = existing.Year,
                Opening = existing.Opening,
                Accrued = existing.Accrued,
                Taken = newTaken,
                CarriedOver = existing.CarriedOver,
                RowVersion = existing.RowVersion
            };
            return Task.FromResult(true);
        }

        return Task.FromResult(false);
    }
}

