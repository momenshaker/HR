namespace HR.Domain.Entities;

/// <summary>
///     Represents a leave request raised by an employee.
/// </summary>
public sealed class LeaveRequest
{
    public Guid Id { get; init; }

    public Guid EmployeeId { get; init; }

    public Guid LeaveTypeId { get; init; }

    public string LeaveType { get; init; } = string.Empty;

    public DateOnly StartDate { get; init; }

    public DateOnly EndDate { get; init; }

    public decimal NumberOfDays { get; init; }

    public string Status { get; init; } = string.Empty;

    public Guid? ApproverId { get; init; }

    public string Reason { get; init; } = string.Empty;

    public string? AttachmentPath { get; init; }

    public DateTime SubmittedAtUtc { get; init; }

    public DateTime? ApprovedAtUtc { get; init; }

    public DateTime? RejectedAtUtc { get; init; }

    public DateTime? CancelledAtUtc { get; init; }
}
