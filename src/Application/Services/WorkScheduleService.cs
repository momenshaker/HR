using HR.Application.Abstractions.Repositories;
using HR.Application.Abstractions.Services;
using HR.Application.DTOs;
using HR.Application.Mappings;

namespace HR.Application.Services;

/// <inheritdoc />
public sealed class WorkScheduleService : IWorkScheduleService
{
    private readonly IWorkScheduleRepository _repository;

    public WorkScheduleService(IWorkScheduleRepository repository)
    {
        _repository = repository;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<WorkScheduleDto>> GetAsync(CancellationToken cancellationToken = default)
    {
        var schedules = await _repository.GetAllAsync(cancellationToken).ConfigureAwait(false);
        return schedules.Select(schedule => schedule.ToDto()).ToArray();
    }

    /// <inheritdoc />
    public async Task<WorkScheduleDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var schedule = await _repository.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
        return schedule?.ToDto();
    }

    /// <inheritdoc />
    public async Task<WorkScheduleDto> CreateAsync(CreateWorkScheduleRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var entity = request.ToEntity();
        var created = await _repository.AddAsync(entity, cancellationToken).ConfigureAwait(false);
        return created.ToDto();
    }

    /// <inheritdoc />
    public async Task<WorkScheduleDto?> UpdateAsync(Guid id, UpdateWorkScheduleRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var existing = await _repository.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
        if (existing is null)
        {
            return null;
        }

        var entity = request.ApplyUpdates(existing);
        var persisted = await _repository.UpdateAsync(entity, cancellationToken).ConfigureAwait(false);
        return persisted?.ToDto();
    }

    /// <inheritdoc />
    public Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return _repository.RemoveAsync(id, cancellationToken);
    }
}
