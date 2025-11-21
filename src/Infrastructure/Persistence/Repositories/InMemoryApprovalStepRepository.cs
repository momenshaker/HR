using System.Collections.Concurrent;
using HR.Application.Abstractions.Repositories;
using HR.Domain.Entities;

namespace HR.Infrastructure.Persistence.Repositories;

public sealed class InMemoryApprovalStepRepository : IApprovalStepRepository
{
    private readonly ConcurrentDictionary<Guid, ApprovalStep> _steps = new();

    public Task<IReadOnlyCollection<ApprovalStep>> GetByLeaveRequestIdAsync(Guid leaveRequestId, CancellationToken cancellationToken = default)
    {
        var steps = _steps.Values
            .Where(s => s.LeaveRequestId == leaveRequestId)
            .OrderBy(s => s.StepOrder)
            .ToList();

        return Task.FromResult<IReadOnlyCollection<ApprovalStep>>(steps);
    }

    public Task<IReadOnlyCollection<ApprovalStep>> GetByApproverAsync(Guid approverId, CancellationToken cancellationToken = default)
    {
        var steps = _steps.Values
            .Where(s => s.ApproverId == approverId)
            .ToList();

        return Task.FromResult<IReadOnlyCollection<ApprovalStep>>(steps);
    }

    public Task<IReadOnlyCollection<ApprovalStep>> ReplaceWorkflowAsync(Guid leaveRequestId, IReadOnlyCollection<ApprovalStep> steps, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(steps);

        var toRemove = _steps.Values.Where(s => s.LeaveRequestId == leaveRequestId).Select(s => s.Id).ToList();
        foreach (var id in toRemove)
        {
            _steps.TryRemove(id, out _);
        }

        foreach (var step in steps)
        {
            if (!_steps.TryAdd(step.Id, step))
            {
                throw new InvalidOperationException($"An approval step with id '{step.Id}' already exists.");
            }
        }

        return Task.FromResult<IReadOnlyCollection<ApprovalStep>>(steps.ToArray());
    }

    public Task<ApprovalStep?> UpdateAsync(ApprovalStep step, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(step);

        if (!_steps.ContainsKey(step.Id))
        {
            return Task.FromResult<ApprovalStep?>(null);
        }

        _steps[step.Id] = step;
        return Task.FromResult<ApprovalStep?>(step);
    }
}
