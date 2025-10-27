using HR.Domain.Entities;

namespace HR.Application.Abstractions.Repositories;

/// <summary>
///     Repository contract for managing <see cref="LeaveRequest" /> aggregates.
/// </summary>
public interface ILeaveRequestRepository
{
    Task<IReadOnlyCollection<LeaveRequest>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<LeaveRequest?> GetByIdAsync(Guid leaveRequestId, CancellationToken cancellationToken = default);

    Task<LeaveRequest> AddAsync(LeaveRequest leaveRequest, CancellationToken cancellationToken = default);

    Task<LeaveRequest?> UpdateAsync(LeaveRequest leaveRequest, CancellationToken cancellationToken = default);

    Task<bool> RemoveAsync(Guid leaveRequestId, CancellationToken cancellationToken = default);
}
