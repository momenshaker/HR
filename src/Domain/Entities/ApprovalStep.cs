namespace HR.Domain.Entities;

/// <summary>
///     Represents a single approval step within a leave workflow.
/// </summary>
public sealed class ApprovalStep
{
    public Guid Id { get; init; }

    public Guid LeaveRequestId { get; init; }

    public int StepOrder { get; init; }

    public Guid ApproverId { get; init; }

    public string Status { get; init; } = string.Empty;

    public DateTime? ActionAtUtc { get; init; }

    public string Comment { get; init; } = string.Empty;
}
