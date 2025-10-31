using System;

namespace HR.Domain.Entities;

public sealed class TimesheetEntry
{
    public Guid Id { get; init; }
    public Guid TimesheetId { get; init; }
    public DateOnly DateUtc { get; init; }
    public Guid? DepartmentId { get; init; }
    public string? ProjectCode { get; init; }
    public string? TaskCode { get; init; }
    public decimal Hours { get; init; }
    public string? Description { get; init; }
    public byte[]? RowVersion { get; init; }

    public Timesheet? Timesheet { get; init; }
    public Department? Department { get; init; }
}

