using HR.Application.Abstractions.Repositories;
using HR.Application.Abstractions.Services;
using HR.Application.DTOs;
using HR.Application.Mappings;

namespace HR.Application.Services;

/// <inheritdoc />
public sealed class PositionService : IPositionService
{
    private readonly IPositionRepository _positionRepository;

    public PositionService(IPositionRepository positionRepository)
    {
        _positionRepository = positionRepository;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<PositionDto>> GetAsync(CancellationToken cancellationToken = default)
    {
        var positions = await _positionRepository.GetAllAsync(cancellationToken).ConfigureAwait(false);
        return positions.Select(position => position.ToDto()).ToArray();
    }

    /// <inheritdoc />
    public async Task<PositionDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var position = await _positionRepository.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
        return position?.ToDto();
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<PositionDto>> GetByOrganizationUnitAsync(
        Guid organizationUnitId,
        CancellationToken cancellationToken = default)
    {
        var positions = await _positionRepository
            .GetByOrganizationUnitAsync(organizationUnitId, cancellationToken)
            .ConfigureAwait(false);

        return positions.Select(position => position.ToDto()).ToArray();
    }

    /// <inheritdoc />
    public async Task<PositionDto> CreateAsync(CreatePositionRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var entity = request.ToEntity();
        var created = await _positionRepository.AddAsync(entity, cancellationToken).ConfigureAwait(false);
        return created.ToDto();
    }

    /// <inheritdoc />
    public async Task<PositionDto?> UpdateAsync(
        Guid id,
        UpdatePositionRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var existing = await _positionRepository.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
        if (existing is null)
        {
            return null;
        }

        var updatedEntity = request.ApplyUpdates(existing);
        var persisted = await _positionRepository.UpdateAsync(updatedEntity, cancellationToken).ConfigureAwait(false);
        return persisted?.ToDto();
    }

    /// <inheritdoc />
    public Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return _positionRepository.RemoveAsync(id, cancellationToken);
    }
}
