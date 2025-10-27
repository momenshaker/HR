using HR.Application.Abstractions.Repositories;
using HR.Application.Abstractions.Services;
using HR.Application.DTOs;
using HR.Application.Mappings;

namespace HR.Application.Services;

/// <inheritdoc />
public sealed class LeaveManagementService : ILeaveManagementService
{
    private readonly ILeaveRequestRepository _leaveRepository;

    public LeaveManagementService(ILeaveRequestRepository leaveRepository)
    {
        _leaveRepository = leaveRepository;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<LeaveRequestDto>> GetAsync(CancellationToken cancellationToken = default)
    {
        var leaves = await _leaveRepository.GetAllAsync(cancellationToken).ConfigureAwait(false);
        return leaves.Select(leave => leave.ToDto()).ToArray();
    }

    /// <inheritdoc />
    public async Task<LeaveRequestDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var leave = await _leaveRepository.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
        return leave?.ToDto();
    }

    /// <inheritdoc />
    public async Task<LeaveRequestDto> CreateAsync(CreateLeaveRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var entity = request.ToEntity();
        var created = await _leaveRepository.AddAsync(entity, cancellationToken).ConfigureAwait(false);

        return created.ToDto();
    }

    /// <inheritdoc />
    public async Task<LeaveRequestDto?> UpdateAsync(Guid id, UpdateLeaveRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var existing = await _leaveRepository.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
        if (existing is null)
        {
            return null;
        }

        var updatedEntity = request.ApplyUpdates(existing);
        var persisted = await _leaveRepository.UpdateAsync(updatedEntity, cancellationToken).ConfigureAwait(false);

        return persisted?.ToDto();
    }

    /// <inheritdoc />
    public Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return _leaveRepository.RemoveAsync(id, cancellationToken);
    }
}
