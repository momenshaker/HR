using HR.Application.Abstractions.Repositories;
using HR.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HR.Infrastructure.Persistence.EntityFramework.Repositories;

internal sealed class EntityFrameworkApprovalStepRepository : EntityFrameworkRepository<ApprovalStep>, IApprovalStepRepository
{
    public EntityFrameworkApprovalStepRepository(HrDbContext dbContext)
        : base(dbContext, step => step.Id)
    {
    }

    public async Task<IReadOnlyCollection<ApprovalStep>> GetByLeaveRequestIdAsync(Guid leaveRequestId, CancellationToken cancellationToken = default)
    {
        var steps = await DbContext.ApprovalSteps.AsNoTracking()
            .Where(s => s.LeaveRequestId == leaveRequestId)
            .OrderBy(s => s.StepOrder)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        return steps;
    }

    public async Task<IReadOnlyCollection<ApprovalStep>> GetByApproverAsync(Guid approverId, CancellationToken cancellationToken = default)
    {
        var steps = await DbContext.ApprovalSteps.AsNoTracking()
            .Where(s => s.ApproverId == approverId)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        return steps;
    }

    public async Task<IReadOnlyCollection<ApprovalStep>> ReplaceWorkflowAsync(Guid leaveRequestId, IReadOnlyCollection<ApprovalStep> steps, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(steps);

        var existing = await DbContext.ApprovalSteps
            .Where(s => s.LeaveRequestId == leaveRequestId)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        if (existing.Count > 0)
        {
            DbContext.ApprovalSteps.RemoveRange(existing);
        }

        await DbContext.ApprovalSteps.AddRangeAsync(steps, cancellationToken).ConfigureAwait(false);
        await DbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        foreach (var step in steps)
        {
            Detach(step);
        }

        return steps.ToArray();
    }

    public Task<ApprovalStep?> UpdateAsync(ApprovalStep step, CancellationToken cancellationToken = default)
    {
        return UpdateInternalAsync(step, cancellationToken);
    }
}
