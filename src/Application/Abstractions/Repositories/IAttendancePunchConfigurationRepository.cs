using HR.Domain.Entities;

namespace HR.Application.Abstractions.Repositories;

public interface IAttendancePunchConfigurationRepository
{
    Task<IReadOnlyCollection<AttendancePunchConfiguration>> GetActiveAsync(CancellationToken cancellationToken = default);
    Task<AttendancePunchConfiguration?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<AttendancePunchConfiguration> AddAsync(AttendancePunchConfiguration entity, CancellationToken cancellationToken = default);
    Task<AttendancePunchConfiguration?> UpdateAsync(AttendancePunchConfiguration entity, CancellationToken cancellationToken = default);
}
