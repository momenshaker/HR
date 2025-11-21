using HR.Domain.Entities;

namespace HR.Application.Abstractions.Repositories;

/// <summary>
///     Repository contract for managing leave approval workflow steps.
/// </summary>
public interface IApprovalStepRepository
{
    Task<IReadOnlyCollection<ApprovalStep>> GetByLeaveRequestIdAsync(Guid leaveRequestId, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<ApprovalStep>> GetByApproverAsync(Guid approverId, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<ApprovalStep>> ReplaceWorkflowAsync(Guid leaveRequestId, IReadOnlyCollection<ApprovalStep> steps, CancellationToken cancellationToken = default);

    Task<ApprovalStep?> UpdateAsync(ApprovalStep step, CancellationToken cancellationToken = default);
}
