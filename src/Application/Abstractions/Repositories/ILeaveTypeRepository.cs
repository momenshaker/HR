using HR.Domain.Entities;

namespace HR.Application.Abstractions.Repositories;

public interface ILeaveTypeRepository
{
    Task<IReadOnlyCollection<LeaveType>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<LeaveType?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<LeaveType> AddAsync(LeaveType entity, CancellationToken cancellationToken = default);

    Task<LeaveType?> UpdateAsync(LeaveType entity, CancellationToken cancellationToken = default);

    Task<bool> RemoveAsync(Guid id, CancellationToken cancellationToken = default);
}

