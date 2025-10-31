using System.Linq;
using FluentValidation;
using FluentValidation.Results;
using HR.Application.Abstractions.Repositories;
using HR.Application.Abstractions.Services;
using HR.Application.Common;
using HR.Application.Common.Exceptions;
using HR.Application.DTOs;
using HR.Domain.Entities;

namespace HR.Application.Services;

public sealed class TimesheetService : ITimesheetService
{
    private readonly ITimesheetRepository _timesheetRepository;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IDepartmentRepository _departmentRepository;
    private readonly IEmployeeDepartmentRepository _employeeDepartmentRepository;

    public TimesheetService(
        ITimesheetRepository timesheetRepository,
        IEmployeeRepository employeeRepository,
        IDepartmentRepository departmentRepository,
        IEmployeeDepartmentRepository employeeDepartmentRepository)
    {
        _timesheetRepository = timesheetRepository ?? throw new ArgumentNullException(nameof(timesheetRepository));
        _employeeRepository = employeeRepository ?? throw new ArgumentNullException(nameof(employeeRepository));
        _departmentRepository = departmentRepository ?? throw new ArgumentNullException(nameof(departmentRepository));
        _employeeDepartmentRepository = employeeDepartmentRepository ?? throw new ArgumentNullException(nameof(employeeDepartmentRepository));
    }

    public async Task<TimesheetDto> GetWeekAsync(Guid employeeId, DateOnly weekStartUtc, CancellationToken cancellationToken = default)
    {
        if (employeeId == Guid.Empty) throw new ArgumentException("Employee id required.", nameof(employeeId));

        var employee = await _employeeRepository.GetByIdAsync(employeeId, cancellationToken).ConfigureAwait(false)
                      ?? throw new ValidationException(new[] { new ValidationFailure("employeeId", "Employee not found.") });

        var normalizedWeekStart = NormalizeWeekStart(weekStartUtc);
        var existing = await _timesheetRepository.GetByEmployeeWeekAsync(employeeId, normalizedWeekStart, cancellationToken).ConfigureAwait(false);

        if (existing is null)
        {
            var created = new Timesheet
            {
                Id = Guid.NewGuid(),
                EmployeeId = employee.Id,
                WeekStartUtc = normalizedWeekStart,
                Status = TimesheetStatus.Draft
            };

            try
            {
                existing = await _timesheetRepository.AddAsync(created, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception)
            {
                // In case of race to create, re-fetch
                existing = await _timesheetRepository.GetByEmployeeWeekAsync(employeeId, normalizedWeekStart, cancellationToken).ConfigureAwait(false);
                if (existing is null)
                {
                    throw new UniqueConstraintViolationException("Timesheet", "(EmployeeId, WeekStartUtc)", $"{employeeId},{normalizedWeekStart}");
                }
            }
        }

        return ToDto(existing!);
    }

    public async Task<TimesheetEntryDto> UpsertEntryAsync(Guid timesheetId, UpsertTimesheetEntryRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        var timesheet = await RequireTimesheet(timesheetId, cancellationToken).ConfigureAwait(false);

        if (timesheet.Status != TimesheetStatus.Draft && timesheet.Status != TimesheetStatus.Rejected)
        {
            throw new ValidationException(new[] { new ValidationFailure("status", "Timesheet cannot be edited after submission.") });
        }

        EnsureDateInWeek(timesheet.WeekStartUtc, request.DateUtc);
        EnsureHoursRange(request.Hours);

        // Department/org validation
        if (request.DepartmentId.HasValue)
        {
            var entryDept = await _departmentRepository.GetByIdAsync(request.DepartmentId.Value, cancellationToken).ConfigureAwait(false);
            if (entryDept is null)
            {
                throw new ValidationException(new[] { new ValidationFailure("DepartmentId", "Department not found.", "NotFound") });
            }

            var employeeDeptIds = await _employeeDepartmentRepository.GetDepartmentIdsByEmployeeAsync(timesheet.EmployeeId, cancellationToken).ConfigureAwait(false);
            if (employeeDeptIds.Count == 0)
            {
                throw new ValidationException(new[] { new ValidationFailure("DepartmentId", "Employee is not assigned to any department.") });
            }

            var employeeDepartments = await _departmentRepository.GetByIdsAsync(employeeDeptIds, cancellationToken).ConfigureAwait(false);
            var employeeOrgIds = employeeDepartments.Select(d => d.OrganizationId).ToHashSet();
            if (!employeeOrgIds.Contains(entryDept.OrganizationId))
            {
                throw new ValidationException(new[] { new ValidationFailure("DepartmentId", "Department must belong to the same organization as the employee.", "OrgMismatch") });
            }
        }

        // Hours cap per day <= 24
        var others = timesheet.Entries.Where(e => e.DateUtc == request.DateUtc && e.Id != request.Id.GetValueOrDefault());
        var otherHours = others.Sum(e => e.Hours);
        if (otherHours + request.Hours > 24m)
        {
            throw new ValidationException(new[] { new ValidationFailure("Hours", "Total hours for the day cannot exceed 24.", "DailyCapExceeded") });
        }

        var updatedEntries = timesheet.Entries.ToDictionary(e => e.Id);

        if (request.Id.HasValue && updatedEntries.TryGetValue(request.Id.Value, out var existing))
        {
            var updated = new TimesheetEntry
            {
                Id = existing.Id,
                TimesheetId = timesheet.Id,
                DateUtc = request.DateUtc,
                DepartmentId = request.DepartmentId,
                ProjectCode = TrimOrNull(request.ProjectCode),
                TaskCode = TrimOrNull(request.TaskCode),
                Hours = decimal.Round(request.Hours, 2, MidpointRounding.AwayFromZero),
                Description = TrimOrNull(request.Description),
                RowVersion = existing.RowVersion
            };
            updatedEntries[existing.Id] = updated;
        }
        else
        {
            var id = request.Id.GetValueOrDefault(Guid.NewGuid());
            var added = new TimesheetEntry
            {
                Id = id,
                TimesheetId = timesheet.Id,
                DateUtc = request.DateUtc,
                DepartmentId = request.DepartmentId,
                ProjectCode = TrimOrNull(request.ProjectCode),
                TaskCode = TrimOrNull(request.TaskCode),
                Hours = decimal.Round(request.Hours, 2, MidpointRounding.AwayFromZero),
                Description = TrimOrNull(request.Description)
            };
            updatedEntries[id] = added;
        }

        var updatedTimesheet = new Timesheet
        {
            Id = timesheet.Id,
            EmployeeId = timesheet.EmployeeId,
            WeekStartUtc = timesheet.WeekStartUtc,
            Status = timesheet.Status,
            SubmittedAtUtc = timesheet.SubmittedAtUtc,
            ApprovedAtUtc = timesheet.ApprovedAtUtc,
            ManagerId = timesheet.ManagerId,
            Notes = timesheet.Notes,
            RowVersion = timesheet.RowVersion,
            Entries = updatedEntries.Values.ToList()
        };

        var persisted = await _timesheetRepository.UpdateAsync(updatedTimesheet, cancellationToken).ConfigureAwait(false)
                       ?? throw new ValidationException(new[] { new ValidationFailure("timesheetId", "Timesheet not found.") });

        return ToEntryDto(persisted.Entries.First(e => e.DateUtc == request.DateUtc && e.Hours == decimal.Round(request.Hours, 2)));
    }

    public async Task<TimesheetDto?> SubmitAsync(Guid timesheetId, CancellationToken cancellationToken = default)
    {
        var timesheet = await RequireTimesheet(timesheetId, cancellationToken).ConfigureAwait(false);
        timesheet.Submit(DateTimeOffset.UtcNow);
        var updated = await _timesheetRepository.UpdateAsync(timesheet, cancellationToken).ConfigureAwait(false);
        return updated is null ? null : ToDto(updated);
    }

    public async Task<TimesheetDto?> ApproveAsync(Guid timesheetId, Guid managerId, string? notes, CancellationToken cancellationToken = default)
    {
        var timesheet = await RequireTimesheet(timesheetId, cancellationToken).ConfigureAwait(false);
        timesheet.Approve(managerId, notes, DateTimeOffset.UtcNow);
        var updated = await _timesheetRepository.UpdateAsync(timesheet, cancellationToken).ConfigureAwait(false);
        return updated is null ? null : ToDto(updated);
    }

    public async Task<TimesheetDto?> RejectAsync(Guid timesheetId, Guid managerId, string reason, CancellationToken cancellationToken = default)
    {
        var timesheet = await RequireTimesheet(timesheetId, cancellationToken).ConfigureAwait(false);
        if (string.IsNullOrWhiteSpace(reason))
        {
            throw new ValidationException(new[] { new ValidationFailure("reason", "Rejection reason is required.") });
        }
        timesheet.Reject(managerId, reason.Trim());
        var updated = await _timesheetRepository.UpdateAsync(timesheet, cancellationToken).ConfigureAwait(false);
        return updated is null ? null : ToDto(updated);
    }

    public async Task<PaginatedResponse<TimesheetDto>> GetApprovalsAsync(Guid managerId, TimesheetStatus? status, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var items = await _timesheetRepository.GetApprovalsAsync(managerId, status, page, pageSize, cancellationToken).ConfigureAwait(false);
        var total = items.Count; // simple; could be improved
        return new PaginatedResponse<TimesheetDto>(page, pageSize, total, items.Select(ToDto).ToArray());
    }

    private static void EnsureHoursRange(decimal hours)
    {
        if (hours < 0m || hours > 24m)
        {
            throw new ValidationException(new[] { new ValidationFailure("Hours", "Hours must be between 0 and 24.", "InvalidRange") });
        }
    }

    private static void EnsureDateInWeek(DateOnly weekStart, DateOnly date)
    {
        var end = weekStart.AddDays(6);
        if (date < weekStart || date > end)
        {
            throw new ValidationException(new[] { new ValidationFailure("DateUtc", "Entry date must be within the timesheet week.", "OutOfWeek") });
        }
    }

    private static string? TrimOrNull(string? value)
    {
        var trimmed = value?.Trim();
        return string.IsNullOrWhiteSpace(trimmed) ? null : trimmed;
    }

    private async Task<Timesheet> RequireTimesheet(Guid id, CancellationToken cancellationToken)
    {
        var timesheet = await _timesheetRepository.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
        if (timesheet is null)
        {
            throw new ValidationException(new[] { new ValidationFailure("timesheetId", "Timesheet not found.") });
        }
        return timesheet;
    }

    private static DateOnly NormalizeWeekStart(DateOnly anyDay)
    {
        // Assuming Monday-based week. Adjust if Sunday is required.
        var dayOfWeek = (int)anyDay.DayOfWeek;
        var mondayBased = dayOfWeek == 0 ? 6 : dayOfWeek - 1; // Monday=0
        return anyDay.AddDays(-mondayBased);
    }

    private static TimesheetDto ToDto(Timesheet t)
    {
        return new TimesheetDto(
            t.Id,
            t.EmployeeId,
            t.WeekStartUtc,
            t.Status,
            t.SubmittedAtUtc,
            t.ApprovedAtUtc,
            t.ManagerId,
            t.Notes,
            t.Entries.OrderBy(e => e.DateUtc).ThenBy(e => e.ProjectCode).Select(ToEntryDto).ToArray());
    }

    private static TimesheetEntryDto ToEntryDto(TimesheetEntry e)
    {
        return new TimesheetEntryDto(e.Id, e.TimesheetId, e.DateUtc, e.DepartmentId, e.ProjectCode, e.TaskCode, e.Hours, e.Description);
    }
}

