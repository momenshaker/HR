using HR.Domain.Entities;

namespace HR.Application.DTOs;

public sealed record TimesheetDto(
    Guid Id,
    Guid EmployeeId,
    DateOnly WeekStartUtc,
    TimesheetStatus Status,
    DateTimeOffset? SubmittedAtUtc,
    DateTimeOffset? ApprovedAtUtc,
    Guid? ManagerId,
    string? Notes,
    IReadOnlyCollection<TimesheetEntryDto> Entries);

public sealed record TimesheetEntryDto(
    Guid Id,
    Guid TimesheetId,
    DateOnly DateUtc,
    Guid? DepartmentId,
    string? ProjectCode,
    string? TaskCode,
    decimal Hours,
    string? Description);

public sealed class UpsertTimesheetEntryRequest
{
    public Guid? Id { get; init; }
    public DateOnly DateUtc { get; init; }
    public Guid? DepartmentId { get; init; }
    public string? ProjectCode { get; init; }
    public string? TaskCode { get; init; }
    public decimal Hours { get; init; }
    public string? Description { get; init; }
}

public sealed record ApproveTimesheetRequest(Guid ManagerId, string? Notes);
public sealed record RejectTimesheetRequest(Guid ManagerId, string Reason);

