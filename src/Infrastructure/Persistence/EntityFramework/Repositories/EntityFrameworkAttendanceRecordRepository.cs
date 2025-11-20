using HR.Application.Abstractions.Repositories;
using HR.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HR.Infrastructure.Persistence.EntityFramework.Repositories;

internal sealed class EntityFrameworkAttendanceRecordRepository : EntityFrameworkRepository<AttendanceRecord>, IAttendanceRecordRepository
{
    public EntityFrameworkAttendanceRecordRepository(HrDbContext dbContext)
        : base(dbContext, record => record.Id)
    {
    }

    public async Task<IReadOnlyCollection<AttendanceRecord>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await DbContext.AttendanceRecords
            .AsNoTracking()
            .Include(record => record.Punches)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<AttendanceRecord?> GetByIdAsync(Guid attendanceRecordId, CancellationToken cancellationToken = default)
    {
        return await DbContext.AttendanceRecords
            .AsNoTracking()
            .Include(record => record.Punches)
            .FirstOrDefaultAsync(record => record.Id == attendanceRecordId, cancellationToken)
            .ConfigureAwait(false);
    }

    public Task<AttendanceRecord> AddAsync(AttendanceRecord attendanceRecord, CancellationToken cancellationToken = default)
    {
        return AddInternalAsync(attendanceRecord, cancellationToken);
    }

    public async Task<AttendanceRecord?> UpdateAsync(AttendanceRecord attendanceRecord, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(attendanceRecord);

        var tracked = await DbContext.AttendanceRecords
            .Include(record => record.Punches)
            .FirstOrDefaultAsync(record => record.Id == attendanceRecord.Id, cancellationToken)
            .ConfigureAwait(false);

        if (tracked is null)
        {
            return null;
        }

        var entry = DbContext.Entry(tracked);
        entry.CurrentValues.SetValues(attendanceRecord);

        tracked.Punches.Clear();
        foreach (var punch in attendanceRecord.Punches)
        {
            if (!DbContext.AttendancePunches.Any(x => x.Id == punch.Id))
            {
                DbContext.AttendancePunches.Add(punch);
            }
        }
        entry.State = EntityState.Modified;
        await DbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        Detach(tracked);

        return tracked;
    }

    public Task<bool> RemoveAsync(Guid attendanceRecordId, CancellationToken cancellationToken = default)
    {
        return RemoveInternalAsync(attendanceRecordId, cancellationToken);
    }
}
