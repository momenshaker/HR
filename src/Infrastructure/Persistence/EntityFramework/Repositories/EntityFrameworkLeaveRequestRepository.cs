using HR.Application.Abstractions.Repositories;
using HR.Domain.Entities;

namespace HR.Infrastructure.Persistence.EntityFramework.Repositories;

internal sealed class EntityFrameworkLeaveRequestRepository : EntityFrameworkRepository<LeaveRequest>, ILeaveRequestRepository
{
    public EntityFrameworkLeaveRequestRepository(HrDbContext dbContext)
        : base(dbContext, request => request.Id)
    {
    }

    public async Task<IReadOnlyCollection<LeaveRequest>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await GetAllInternalAsync(cancellationToken).ConfigureAwait(false);
    }

    public Task<LeaveRequest?> GetByIdAsync(Guid leaveRequestId, CancellationToken cancellationToken = default)
    {
        return GetByIdInternalAsync(leaveRequestId, cancellationToken);
    }

    public Task<LeaveRequest> AddAsync(LeaveRequest leaveRequest, CancellationToken cancellationToken = default)
    {
        return AddInternalAsync(leaveRequest, cancellationToken);
    }

    public Task<LeaveRequest?> UpdateAsync(LeaveRequest leaveRequest, CancellationToken cancellationToken = default)
    {
        return UpdateInternalAsync(leaveRequest, cancellationToken);
    }

    public Task<bool> RemoveAsync(Guid leaveRequestId, CancellationToken cancellationToken = default)
    {
        return RemoveInternalAsync(leaveRequestId, cancellationToken);
    }
}
