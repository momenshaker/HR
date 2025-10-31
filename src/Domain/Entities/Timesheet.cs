using System;
using System.Collections.Generic;

namespace HR.Domain.Entities;

public sealed class Timesheet
{
    public Guid Id { get; init; }
    public Guid EmployeeId { get; init; }
    public DateOnly WeekStartUtc { get; init; }
    public TimesheetStatus Status { get; set; } = TimesheetStatus.Draft;
    public DateTimeOffset? SubmittedAtUtc { get; set; }
    public DateTimeOffset? ApprovedAtUtc { get; set; }
    public Guid? ManagerId { get; set; }
    public string? Notes { get; set; }
    public byte[]? RowVersion { get; init; }

    public ICollection<TimesheetEntry> Entries { get; set; } = new List<TimesheetEntry>();

    public void Submit(DateTimeOffset nowUtc)
    {
        if (Status != TimesheetStatus.Draft)
        {
            throw new InvalidOperationException("Only draft timesheets can be submitted.");
        }

        Status = TimesheetStatus.Submitted;
        SubmittedAtUtc = nowUtc;
    }

    public void Approve(Guid managerId, string? notes, DateTimeOffset nowUtc)
    {
        if (Status != TimesheetStatus.Submitted)
        {
            throw new InvalidOperationException("Only submitted timesheets can be approved.");
        }

        Status = TimesheetStatus.Approved;
        ManagerId = managerId;
        Notes = notes ?? Notes;
        ApprovedAtUtc = nowUtc;
    }

    public void Reject(Guid managerId, string reason)
    {
        if (Status != TimesheetStatus.Submitted)
        {
            throw new InvalidOperationException("Only submitted timesheets can be rejected.");
        }

        Status = TimesheetStatus.Rejected;
        ManagerId = managerId;
        Notes = reason;
        ApprovedAtUtc = null;
    }
}
