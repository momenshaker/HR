using System.Collections.Concurrent;
using HR.Application.Abstractions.Repositories;
using HR.Domain.Entities;

namespace HR.Infrastructure.Persistence.Repositories;

/// <summary>
///     In-memory repository implementation for leave requests.
/// </summary>
public sealed class InMemoryLeaveRequestRepository : ILeaveRequestRepository
{
    private readonly ConcurrentDictionary<Guid, LeaveRequest> _leaveRequests = new();

    public Task<IReadOnlyCollection<LeaveRequest>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        IReadOnlyCollection<LeaveRequest> snapshot = _leaveRequests.Values.ToList();
        return Task.FromResult(snapshot);
    }

    public Task<LeaveRequest?> GetByIdAsync(Guid leaveRequestId, CancellationToken cancellationToken = default)
    {
        _leaveRequests.TryGetValue(leaveRequestId, out var leaveRequest);
        return Task.FromResult(leaveRequest);
    }

    public Task<LeaveRequest> AddAsync(LeaveRequest leaveRequest, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(leaveRequest);

        if (!_leaveRequests.TryAdd(leaveRequest.Id, leaveRequest))
        {
            throw new InvalidOperationException($"A leave request with id '{leaveRequest.Id}' already exists.");
        }

        return Task.FromResult(leaveRequest);
    }

    public Task<LeaveRequest?> UpdateAsync(LeaveRequest leaveRequest, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(leaveRequest);

        if (!_leaveRequests.ContainsKey(leaveRequest.Id))
        {
            return Task.FromResult<LeaveRequest?>(null);
        }

        _leaveRequests[leaveRequest.Id] = leaveRequest;
        return Task.FromResult<LeaveRequest?>(leaveRequest);
    }

    public Task<bool> RemoveAsync(Guid leaveRequestId, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_leaveRequests.TryRemove(leaveRequestId, out _));
    }
}
