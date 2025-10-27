namespace HR.Application.DTOs;

/// <summary>
///     Read model representing a leave request.
/// </summary>
public sealed record LeaveRequestDto(
    Guid Id,
    Guid EmployeeId,
    string LeaveType,
    DateOnly StartDate,
    DateOnly EndDate,
    string Status,
    Guid? ApproverId,
    string Reason,
    DateTime RequestedAtUtc,
    DateTime? DecisionAtUtc);
