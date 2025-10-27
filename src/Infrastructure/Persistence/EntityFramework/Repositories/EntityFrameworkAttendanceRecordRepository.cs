using HR.Application.Abstractions.Repositories;
using HR.Domain.Entities;

namespace HR.Infrastructure.Persistence.EntityFramework.Repositories;

internal sealed class EntityFrameworkAttendanceRecordRepository : EntityFrameworkRepository<AttendanceRecord>, IAttendanceRecordRepository
{
    public EntityFrameworkAttendanceRecordRepository(HrDbContext dbContext)
        : base(dbContext, record => record.Id)
    {
    }

    public async Task<IReadOnlyCollection<AttendanceRecord>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await GetAllInternalAsync(cancellationToken).ConfigureAwait(false);
    }

    public Task<AttendanceRecord?> GetByIdAsync(Guid attendanceRecordId, CancellationToken cancellationToken = default)
    {
        return GetByIdInternalAsync(attendanceRecordId, cancellationToken);
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
