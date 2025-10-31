using System.Collections.Concurrent;
using System.Linq;
using HR.Application.Abstractions.Repositories;
using HR.Domain.Entities;

namespace HR.Infrastructure.Persistence.Repositories;

public sealed class InMemoryTimesheetRepository : ITimesheetRepository
{
    private readonly ConcurrentDictionary<Guid, Timesheet> _timesheets = new();

    public Task<Timesheet?> GetByIdAsync(Guid timesheetId, CancellationToken cancellationToken = default)
    {
        _timesheets.TryGetValue(timesheetId, out var t);
        return Task.FromResult(t);
    }

    public Task<Timesheet?> GetByEmployeeWeekAsync(Guid employeeId, DateOnly weekStartUtc, CancellationToken cancellationToken = default)
    {
        var t = _timesheets.Values.FirstOrDefault(x => x.EmployeeId == employeeId && x.WeekStartUtc == weekStartUtc);
        return Task.FromResult(t);
    }

    public Task<Timesheet> AddAsync(Timesheet timesheet, CancellationToken cancellationToken = default)
    {
        if (!_timesheets.TryAdd(timesheet.Id, Clone(timesheet)))
        {
            throw new InvalidOperationException("Duplicate timesheet id.");
        }
        return Task.FromResult(timesheet);
    }

    public Task<Timesheet?> UpdateAsync(Timesheet timesheet, CancellationToken cancellationToken = default)
    {
        if (!_timesheets.ContainsKey(timesheet.Id))
        {
            return Task.FromResult<Timesheet?>(null);
        }
        _timesheets[timesheet.Id] = Clone(timesheet);
        return Task.FromResult<Timesheet?>(timesheet);
    }

    public Task<IReadOnlyCollection<Timesheet>> GetApprovalsAsync(Guid managerId, TimesheetStatus? status, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var q = _timesheets.Values.AsEnumerable();
        if (status.HasValue) q = q.Where(t => t.Status == status.Value);
        q = q.Where(t => t.Status == TimesheetStatus.Submitted || t.ManagerId == managerId)
             .OrderByDescending(t => t.SubmittedAtUtc);
        var list = q.Skip((page - 1) * pageSize).Take(pageSize).Select(Clone).ToList();
        return Task.FromResult<IReadOnlyCollection<Timesheet>>(list);
    }

    private static Timesheet Clone(Timesheet t)
    {
        return new Timesheet
        {
            Id = t.Id,
            EmployeeId = t.EmployeeId,
            WeekStartUtc = t.WeekStartUtc,
            Status = t.Status,
            SubmittedAtUtc = t.SubmittedAtUtc,
            ApprovedAtUtc = t.ApprovedAtUtc,
            ManagerId = t.ManagerId,
            Notes = t.Notes,
            RowVersion = t.RowVersion,
            Entries = t.Entries.Select(e => new TimesheetEntry
            {
                Id = e.Id,
                TimesheetId = e.TimesheetId,
                DateUtc = e.DateUtc,
                DepartmentId = e.DepartmentId,
                ProjectCode = e.ProjectCode,
                TaskCode = e.TaskCode,
                Hours = e.Hours,
                Description = e.Description,
                RowVersion = e.RowVersion
            }).ToList()
        };
    }
}

