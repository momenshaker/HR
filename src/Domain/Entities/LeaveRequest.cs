namespace HR.Domain.Entities;

/// <summary>
///     Represents a leave request raised by an employee.
/// </summary>
public sealed class LeaveRequest
{
    public Guid Id { get; init; }

    public Guid EmployeeId { get; init; }

    public string LeaveType { get; init; } = string.Empty;

    public DateOnly StartDate { get; init; }

    public DateOnly EndDate { get; init; }

    public string Status { get; init; } = string.Empty;

    public Guid? ApproverId { get; init; }

    public string Reason { get; init; } = string.Empty;

    public DateTime RequestedAtUtc { get; init; }

    public DateTime? DecisionAtUtc { get; init; }
}
