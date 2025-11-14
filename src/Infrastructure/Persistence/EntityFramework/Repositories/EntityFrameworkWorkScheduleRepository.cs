using HR.Application.Abstractions.Repositories;
using HR.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HR.Infrastructure.Persistence.EntityFramework.Repositories;

internal sealed class EntityFrameworkWorkScheduleRepository : IWorkScheduleRepository
{
    private readonly HrDbContext _dbContext;

    public EntityFrameworkWorkScheduleRepository(HrDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyCollection<WorkSchedule>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.WorkSchedules
            .AsNoTracking()
            .Include(schedule => schedule.ShiftTemplates)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<WorkSchedule?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.WorkSchedules
            .AsNoTracking()
            .Include(schedule => schedule.ShiftTemplates)
            .FirstOrDefaultAsync(schedule => schedule.Id == id, cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<WorkSchedule> AddAsync(WorkSchedule schedule, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(schedule);

        await _dbContext.WorkSchedules.AddAsync(schedule, cancellationToken).ConfigureAwait(false);
        await _dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        DetachGraph(schedule);
        return schedule;
    }

    public async Task<WorkSchedule?> UpdateAsync(WorkSchedule schedule, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(schedule);

        var existing = await _dbContext.WorkSchedules
            .Include(entity => entity.ShiftTemplates)
            .FirstOrDefaultAsync(entity => entity.Id == schedule.Id, cancellationToken)
            .ConfigureAwait(false);

        if (existing is null)
        {
            return null;
        }

        existing.Name = schedule.Name;
        existing.OrganizationId = schedule.OrganizationId;
        existing.DepartmentId = schedule.DepartmentId;
        existing.IsDefaultForOrganization = schedule.IsDefaultForOrganization;
        existing.TimeZoneId = schedule.TimeZoneId;

        var existingTemplates = existing.ShiftTemplates.ToList();
        _dbContext.ShiftTemplates.RemoveRange(existingTemplates);
        existing.ShiftTemplates.Clear();

        foreach (var template in schedule.ShiftTemplates)
        {
            existing.ShiftTemplates.Add(new ShiftTemplate
            {
                Id = template.Id == Guid.Empty ? Guid.NewGuid() : template.Id,
                WorkScheduleId = existing.Id,
                DayOfWeek = template.DayOfWeek,
                StartTime = template.StartTime,
                EndTime = template.EndTime,
                BreakMinutes = template.BreakMinutes,
                GracePeriodMinutes = template.GracePeriodMinutes,
                MinimumOvertimeMinutes = template.MinimumOvertimeMinutes
            });
        }

        await _dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        DetachGraph(existing);
        return existing;
    }

    public async Task<bool> RemoveAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _dbContext.WorkSchedules.FindAsync(new object?[] { id }, cancellationToken).ConfigureAwait(false);
        if (entity is null)
        {
            return false;
        }

        _dbContext.WorkSchedules.Remove(entity);
        await _dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return true;
    }

    private void DetachGraph(WorkSchedule schedule)
    {
        var entry = _dbContext.Entry(schedule);
        if (entry.State != EntityState.Detached)
        {
            entry.State = EntityState.Detached;
        }

        foreach (var template in schedule.ShiftTemplates)
        {
            var templateEntry = _dbContext.Entry(template);
            if (templateEntry.State != EntityState.Detached)
            {
                templateEntry.State = EntityState.Detached;
            }
        }
    }
}
