using System.ComponentModel.DataAnnotations;
using HR.Application.Validation;

namespace HR.Application.DTOs;

/// <summary>
///     Request payload for defining the approval workflow of a leave request.
/// </summary>
public sealed class CreateLeaveApprovalWorkflowRequest : IValidatableRequest
{
    [Required]
    public Guid LeaveRequestId { get; init; }

    [Required]
    public IReadOnlyCollection<LeaveApprovalStepInput> Steps { get; init; } = Array.Empty<LeaveApprovalStepInput>();
}

/// <summary>
///     Represents a single approval step input within a workflow.
/// </summary>
public sealed class LeaveApprovalStepInput
{
    [Required]
    public Guid ApproverId { get; init; }

    /// <summary>
    ///     Optional explicit step order. If omitted, steps are ordered in the same sequence they appear in the request body.
    /// </summary>
    [Range(1, int.MaxValue)]
    public int? StepOrder { get; init; }
}
