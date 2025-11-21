namespace HR.Application.DTOs;

/// <summary>
///     Read model for a single leave approval step.
/// </summary>
public sealed record LeaveApprovalStepDto(
    Guid Id,
    Guid LeaveRequestId,
    int StepOrder,
    Guid ApproverId,
    string Status,
    DateTime? ActionAtUtc,
    string Comment);
