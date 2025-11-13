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

    public Task<AttendanceRecord?> UpdateAsync(AttendanceRecord attendanceRecord, CancellationToken cancellationToken = default)
    {
        return UpdateInternalAsync(attendanceRecord, cancellationToken);
    }

    public Task<bool> RemoveAsync(Guid attendanceRecordId, CancellationToken cancellationToken = default)
    {
        return RemoveInternalAsync(attendanceRecordId, cancellationToken);
    }
}
