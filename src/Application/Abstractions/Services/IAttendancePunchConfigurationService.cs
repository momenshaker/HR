using HR.Application.DTOs;

namespace HR.Application.Abstractions.Services;

public interface IAttendancePunchConfigurationService
{
    Task<IReadOnlyCollection<AttendancePunchConfigurationDto>> GetPunchTypesAsync(CancellationToken cancellationToken = default);
    Task<AttendancePunchConfigurationDto> SaveAsync(AttendancePunchConfigurationRequest request, CancellationToken cancellationToken = default);
}
