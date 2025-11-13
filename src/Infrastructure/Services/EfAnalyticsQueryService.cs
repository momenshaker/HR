using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HR.Application.Abstractions.Services;
using HR.Application.DTOs;
using HR.Domain.Entities;
using HR.Infrastructure.Persistence.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace HR.Infrastructure.Services;

public sealed class EfAnalyticsQueryService : IAnalyticsQueryService
{
    private readonly HrDbContext _db;
    private readonly ILightweightTrainingService _liteTraining;

    public EfAnalyticsQueryService(HrDbContext db, ILightweightTrainingService liteTraining)
    {
        _db = db ?? throw new ArgumentNullException(nameof(db));
        _liteTraining = liteTraining ?? throw new ArgumentNullException(nameof(liteTraining));
    }

    public async Task<IReadOnlyCollection<HeadcountItemDto>> GetHeadcountAsync(Guid organizationId, Guid? departmentId, CancellationToken cancellationToken = default)
    {
        var deptQuery = _db.Departments.AsNoTracking().Where(d => d.OrganizationId == organizationId && d.IsActive);

        string? subtreePath = null;
        if (departmentId.HasValue)
        {
            subtreePath = await deptQuery.Where(d => d.Id == departmentId.Value).Select(d => d.Path).FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false);
            if (string.IsNullOrWhiteSpace(subtreePath))
            {
                return Array.Empty<HeadcountItemDto>();
            }
            deptQuery = deptQuery.Where(d => d.Path.StartsWith(subtreePath));
        }

        var deptIds = await deptQuery.Select(d => new { d.Id, d.Name }).ToListAsync(cancellationToken).ConfigureAwait(false);
        var targetDeptIds = deptIds.Select(x => x.Id).ToHashSet();
        var deptNames = deptIds.ToDictionary(x => x.Id, x => x.Name);

        var activeEmployees = await _db.Employees.AsNoTracking()
            .Where(e => e.IsActive && e.EmploymentStartDate <= DateOnly.FromDateTime(DateTime.UtcNow) && (e.EmploymentEndDate == null || e.EmploymentEndDate > DateOnly.FromDateTime(DateTime.UtcNow)))
            .Select(e => e.Id)
            .ToArrayAsync(cancellationToken)
            .ConfigureAwait(false);
        var activeSet = new HashSet<Guid>(activeEmployees);

        var counts = await _db.EmployeeDepartments.AsNoTracking()
            .Where(ed => targetDeptIds.Contains(ed.DepartmentId) && activeSet.Contains(ed.EmployeeId))
            .GroupBy(ed => ed.DepartmentId)
            .Select(g => new { DepartmentId = g.Key, Count = g.Select(x => x.EmployeeId).Distinct().Count() })
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        return counts
            .Select(c => new HeadcountItemDto(c.DepartmentId, deptNames.TryGetValue(c.DepartmentId, out var name) ? name : string.Empty, c.Count))
            .OrderBy(x => x.DepartmentName, StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }

    public async Task<IReadOnlyCollection<UtilizationPeriodDto>> GetUtilizationAsync(Guid organizationId, DateOnly start, DateOnly end, CancellationToken cancellationToken = default)
    {
        if (end < start) return Array.Empty<UtilizationPeriodDto>();

        var deptIds = await _db.Departments.AsNoTracking()
            .Where(d => d.OrganizationId == organizationId && d.IsActive)
            .Select(d => d.Id)
            .ToArrayAsync(cancellationToken)
            .ConfigureAwait(false);
        var deptSet = new HashSet<Guid>(deptIds);

        // Collect approved entries within range and org departments
        var entries = from t in _db.Timesheets.AsNoTracking()
                      where t.Status == TimesheetStatus.Approved
                      from e in t.Entries
                      where e.DateUtc >= start && e.DateUtc < end && e.DepartmentId != null && deptSet.Contains(e.DepartmentId.Value)
                      select new { e.DateUtc, e.Hours };

        var approvedByWeek = await entries
            .AsQueryable()
            .GroupBy(x => FirstDayOfWeek(x.DateUtc))
            .Select(g => new { WeekStart = g.Key, Hours = g.Sum(x => x.Hours) })
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        // Capacity: 8h per workday per active employee in org per week
        var activeEmployees = await _db.Employees.AsNoTracking()
            .Where(e => e.IsActive)
            .Select(e => new { e.Id, e.EmploymentStartDate, e.EmploymentEndDate })
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        var weeks = WeeksBetweenInclusive(start, end).ToArray();
        var results = new List<UtilizationPeriodDto>(weeks.Length);
        foreach (var tuple in weeks)
        {
            var weekStart = tuple.Item1;
            var weekEnd = tuple.Item2;
            var approved = approvedByWeek.FirstOrDefault(w => w.WeekStart == weekStart)?.Hours ?? 0m;

            var activeCount = activeEmployees.Count(e => e.EmploymentStartDate <= weekEnd && (e.EmploymentEndDate == null || e.EmploymentEndDate > weekStart));
            var workdays = WorkdaysInRange(weekStart, weekEnd);
            var capacity = activeCount * workdays * 8m;
            var rate = capacity > 0 ? Math.Round(approved / capacity, 4) : 0m;
            results.Add(new UtilizationPeriodDto(weekStart, weekEnd, DecimalRound(approved), DecimalRound(capacity), rate));
        }
        return results;
    }

    public async Task<IReadOnlyCollection<LeaveUsageItemDto>> GetLeaveUsageAsync(Guid organizationId, int year, CancellationToken cancellationToken = default)
    {
        var orgDeptIds = await _db.Departments.AsNoTracking()
            .Where(d => d.OrganizationId == organizationId)
            .Select(d => d.Id)
            .ToArrayAsync(cancellationToken)
            .ConfigureAwait(false);
        var membership = await _db.EmployeeDepartments.AsNoTracking()
            .Where(ed => orgDeptIds.Contains(ed.DepartmentId))
            .Select(ed => ed.EmployeeId)
            .Distinct()
            .ToArrayAsync(cancellationToken)
            .ConfigureAwait(false);
        var empSet = new HashSet<Guid>(membership);
        var start = new DateOnly(year, 1, 1);
        var end = new DateOnly(year + 1, 1, 1);

        var usage = await _db.LeaveRequests.AsNoTracking()
            .Where(l => l.Status == LeaveRequestStatus.Approved && empSet.Contains(l.EmployeeId))
            .Where(l => !(l.EndDate < start || l.StartDate >= end))
            .GroupBy(l => l.LeaveType)
            .Select(g => new
            {
                LeaveType = g.Key,
                Days = g.Sum(l => OverlapDays(l.StartDate, l.EndDate, start, end))
            })
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        return usage.Select(x => new LeaveUsageItemDto(x.LeaveType, x.Days)).OrderBy(x => x.LeaveType, StringComparer.OrdinalIgnoreCase).ToArray();
    }

    public async Task<PayrollTotalsResponseDto> GetPayrollTotalsAsync(Guid organizationId, DateOnly start, DateOnly end, CancellationToken cancellationToken = default)
    {
        var runs = await _db.PayrollRuns.AsNoTracking()
            .Where(r => r.OrganizationId == organizationId && r.PeriodStart >= start && r.PeriodEnd < end)
            .Select(r => new { r.Id, r.PeriodStart, r.PeriodEnd, r.TotalGrossPay, r.TotalNetPay })
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        var runIds = runs.Select(r => r.Id).ToArray();

        var runTotals = runs
            .Select(r => new PayrollRunTotalsDto(r.Id, r.PeriodStart, r.PeriodEnd, DecimalRound(r.TotalGrossPay), DecimalRound(r.TotalNetPay)))
            .OrderBy(r => r.PeriodStart)
            .ToArray();

        // Department totals based on payroll items' employees' primary department
        var primaryDeptByEmployee = await _db.EmployeeDepartments.AsNoTracking()
            .Where(ed => ed.IsPrimary)
            .ToDictionaryAsync(x => x.EmployeeId, x => x.DepartmentId, cancellationToken)
            .ConfigureAwait(false);

        var deptNames = await _db.Departments.AsNoTracking()
            .Where(d => d.OrganizationId == organizationId)
            .ToDictionaryAsync(d => d.Id, d => d.Name, cancellationToken)
            .ConfigureAwait(false);

        var items = await _db.PayrollItems.AsNoTracking()
            .Where(i => runIds.Contains(i.RunId))
            .Select(i => new { i.EmployeeId, i.Gross, i.Net })
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        var deptAgg = items
            .Select(i => new
            {
                DepartmentId = primaryDeptByEmployee.TryGetValue(i.EmployeeId, out var dId) ? dId : (Guid?)null,
                i.Gross,
                i.Net
            })
            .Where(x => x.DepartmentId.HasValue)
            .GroupBy(x => x.DepartmentId!.Value)
            .Select(g => new DepartmentPayrollTotalsDto(
                g.Key,
                deptNames.TryGetValue(g.Key, out var name) ? name : string.Empty,
                DecimalRound(g.Sum(x => x.Gross)),
                DecimalRound(g.Sum(x => x.Net))))
            .OrderBy(x => x.DepartmentName, StringComparer.OrdinalIgnoreCase)
            .ToArray();

        return new PayrollTotalsResponseDto(runTotals, deptAgg);
    }

    public async Task<IReadOnlyCollection<StageCountDto>> GetRecruitmentFunnelAsync(Guid vacancyId, CancellationToken cancellationToken = default)
    {
        var counts = await _db.InterviewSchedules.AsNoTracking()
            .Where(i => i.VacancyId == vacancyId)
            .GroupBy(i => i.Stage)
            .Select(g => new { Stage = g.Key, Count = g.Count() })
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        return counts.Select(c => new StageCountDto(c.Stage, c.Count)).OrderBy(x => x.Stage, StringComparer.OrdinalIgnoreCase).ToArray();
    }

    public async Task<TrainingComplianceDto> GetTrainingComplianceAsync(Guid organizationId, CancellationToken cancellationToken = default)
    {
        var gaps = await _liteTraining.GetMandatoryCompletionGapsAsync(organizationId, cancellationToken).ConfigureAwait(false);

        // Derive mandatory course count from any non-empty gap list or 0 if none
        var mandatoryCount = gaps.Values.FirstOrDefault()?.Count ?? 0;
        var observed = gaps.Count; // employees we observed via enrollments/completions
        var compliant = gaps.Count == 0 ? 0 : gaps.Count(kvp => kvp.Value.Count == 0);
        var rate = observed == 0 ? 0m : Math.Round((decimal)compliant / observed, 4);

        return new TrainingComplianceDto(organizationId, mandatoryCount, observed, compliant, rate);
    }

    private static DateOnly FirstDayOfWeek(DateOnly date)
    {
        var delta = (int)date.DayOfWeek; // Monday=1 ... Sunday=0; but DateOnly uses Sunday=0
        var sundayBased = date.AddDays(-delta);
        // normalize to Monday-based week start
        return sundayBased.DayOfWeek == DayOfWeek.Sunday ? sundayBased.AddDays(1) : sundayBased;
    }

    private static int WorkdaysInRange(DateOnly start, DateOnly end)
    {
        var days = 0;
        for (var d = start; d <= end; d = d.AddDays(1))
        {
            if (d.DayOfWeek != DayOfWeek.Saturday && d.DayOfWeek != DayOfWeek.Sunday)
                days++;
        }
        return days;
    }

    private static IEnumerable<Tuple<DateOnly, DateOnly>> WeeksBetweenInclusive(DateOnly start, DateOnly end)
    {
        // Normalize to Monday-based weeks
        var cursor = FirstDayOfWeek(start);
        var finish = FirstDayOfWeek(end);
        while (cursor <= finish)
        {
            var weekEnd = cursor.AddDays(6);
            if (weekEnd > end) weekEnd = end;
            yield return Tuple.Create(cursor, weekEnd);
            cursor = cursor.AddDays(7);
        }
    }

    private static int OverlapDays(DateOnly start, DateOnly end, DateOnly windowStart, DateOnly windowEnd)
    {
        var s = start < windowStart ? windowStart : start;
        var e = end >= windowEnd ? windowEnd.AddDays(-1) : end;
        if (e < s) return 0;
        return (e.DayNumber - s.DayNumber) + 1;
    }

    private static decimal DecimalRound(decimal value) => Math.Round(value, 2);
}
