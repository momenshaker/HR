namespace HR.Application.DTOs;

/// <summary>
///     Read model representing a leave request.
/// </summary>
public sealed record LeaveRequestDto(
    Guid Id,
    Guid EmployeeId,
    Guid LeaveTypeId,
    string LeaveType,
    DateOnly StartDate,
    DateOnly EndDate,
    decimal NumberOfDays,
    string Status,
    Guid? ApproverId,
    string Reason,
    string? AttachmentPath,
    DateTime SubmittedAtUtc,
    DateTime? ApprovedAtUtc,
    DateTime? RejectedAtUtc,
    DateTime? CancelledAtUtc);
