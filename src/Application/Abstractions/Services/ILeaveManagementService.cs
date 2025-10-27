using HR.Application.DTOs;

namespace HR.Application.Abstractions.Services;

/// <summary>
///     Application service responsible for leave management operations.
/// </summary>
public interface ILeaveManagementService
{
    Task<IReadOnlyCollection<LeaveRequestDto>> GetAsync(CancellationToken cancellationToken = default);

    Task<LeaveRequestDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<LeaveRequestDto> CreateAsync(CreateLeaveRequest request, CancellationToken cancellationToken = default);

    Task<LeaveRequestDto?> UpdateAsync(Guid id, UpdateLeaveRequest request, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
