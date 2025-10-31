using HR.Domain.Entities;

namespace HR.Application.Abstractions.Repositories;

public interface ILeaveBalanceRepository
{
    Task<IReadOnlyCollection<LeaveBalance>> GetByEmployeeYearAsync(Guid employeeId, int year, CancellationToken cancellationToken = default);

    Task<LeaveBalance?> GetAsync(Guid employeeId, Guid leaveTypeId, int year, CancellationToken cancellationToken = default);

    Task<LeaveBalance> UpsertAsync(LeaveBalance entity, CancellationToken cancellationToken = default);

    Task<bool> UpdateTakenAsync(Guid employeeId, Guid leaveTypeId, int year, decimal newTaken, byte[]? expectedRowVersion, CancellationToken cancellationToken = default);
}

