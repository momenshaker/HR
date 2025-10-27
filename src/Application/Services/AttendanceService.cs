using HR.Application.Abstractions.Repositories;
using HR.Application.Abstractions.Services;
using HR.Application.DTOs;
using HR.Application.Mappings;

namespace HR.Application.Services;

/// <inheritdoc />
public sealed class AttendanceService : IAttendanceService
{
    private readonly IAttendanceRecordRepository _attendanceRepository;

    public AttendanceService(IAttendanceRecordRepository attendanceRepository)
    {
        _attendanceRepository = attendanceRepository;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<AttendanceRecordDto>> GetAsync(CancellationToken cancellationToken = default)
    {
        var records = await _attendanceRepository.GetAllAsync(cancellationToken).ConfigureAwait(false);
        return records.Select(record => record.ToDto()).ToArray();
    }

    /// <inheritdoc />
    public async Task<AttendanceRecordDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var record = await _attendanceRepository.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
        return record?.ToDto();
    }

    /// <inheritdoc />
    public async Task<AttendanceRecordDto> CreateAsync(CreateAttendanceRecordRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var entity = request.ToEntity();
        var created = await _attendanceRepository.AddAsync(entity, cancellationToken).ConfigureAwait(false);

        return created.ToDto();
    }

    /// <inheritdoc />
    public async Task<AttendanceRecordDto?> UpdateAsync(Guid id, UpdateAttendanceRecordRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var existing = await _attendanceRepository.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
        if (existing is null)
        {
            return null;
        }

        var updatedEntity = request.ApplyUpdates(existing);
        var persisted = await _attendanceRepository.UpdateAsync(updatedEntity, cancellationToken).ConfigureAwait(false);

        return persisted?.ToDto();
    }

    /// <inheritdoc />
    public Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return _attendanceRepository.RemoveAsync(id, cancellationToken);
    }
}
