using HR.Application.DTOs;
using HR.Domain.Entities;

namespace HR.Application.Mappings;

/// <summary>
///     Mapping helpers for leave approval workflow steps.
/// </summary>
public static class ApprovalStepMappings
{
    public static LeaveApprovalStepDto ToDto(this ApprovalStep step)
    {
        ArgumentNullException.ThrowIfNull(step);

        return new LeaveApprovalStepDto(
            step.Id,
            step.LeaveRequestId,
            step.StepOrder,
            step.ApproverId,
            step.Status,
            step.ActionAtUtc,
            step.Comment);
    }
}
