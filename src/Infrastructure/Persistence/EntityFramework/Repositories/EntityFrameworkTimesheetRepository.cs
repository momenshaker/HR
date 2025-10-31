using System.Linq;
using HR.Application.Abstractions.Repositories;
using HR.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HR.Infrastructure.Persistence.EntityFramework.Repositories;

internal sealed class EntityFrameworkTimesheetRepository : ITimesheetRepository
{
    private readonly HrDbContext _db;

    public EntityFrameworkTimesheetRepository(HrDbContext db)
    {
        _db = db ?? throw new ArgumentNullException(nameof(db));
    }

    public async Task<Timesheet?> GetByIdAsync(Guid timesheetId, CancellationToken cancellationToken = default)
    {
        return await _db.Timesheets
            .AsNoTracking()
            .Include(t => t.Entries)
            .FirstOrDefaultAsync(t => t.Id == timesheetId, cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<Timesheet?> GetByEmployeeWeekAsync(Guid employeeId, DateOnly weekStartUtc, CancellationToken cancellationToken = default)
    {
        return await _db.Timesheets
            .AsNoTracking()
            .Include(t => t.Entries)
            .FirstOrDefaultAsync(t => t.EmployeeId == employeeId && t.WeekStartUtc == weekStartUtc, cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<Timesheet> AddAsync(Timesheet timesheet, CancellationToken cancellationToken = default)
    {
        await _db.Timesheets.AddAsync(timesheet, cancellationToken).ConfigureAwait(false);
        await _db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        _db.Entry(timesheet).State = EntityState.Detached;
        return timesheet;
    }

    public async Task<Timesheet?> UpdateAsync(Timesheet timesheet, CancellationToken cancellationToken = default)
    {
        var exists = await _db.Timesheets.AnyAsync(t => t.Id == timesheet.Id, cancellationToken).ConfigureAwait(false);
        if (!exists) return null;

        _db.Timesheets.Update(timesheet);
        await _db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        _db.Entry(timesheet).State = EntityState.Detached;
        return timesheet;
    }

    public async Task<IReadOnlyCollection<Timesheet>> GetApprovalsAsync(
        Guid managerId,
        TimesheetStatus? status,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = _db.Timesheets.AsNoTracking().Include(t => t.Entries)
            .Where(t => t.ManagerId == managerId || t.Status == TimesheetStatus.Submitted);

        if (status.HasValue)
        {
            query = query.Where(t => t.Status == status.Value);
        }

        return await query
            .OrderByDescending(t => t.SubmittedAtUtc)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }
}

