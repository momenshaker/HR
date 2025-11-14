using HR.Application.Abstractions.Repositories;
using HR.Application.Abstractions.Services;
using HR.Application.DTOs;
using HR.Application.Mappings;

namespace HR.Application.Services;

/// <inheritdoc />
public sealed class HolidayService : IHolidayService
{
    private readonly IHolidayRepository _repository;

    public HolidayService(IHolidayRepository repository)
    {
        _repository = repository;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<HolidayDto>> GetAsync(CancellationToken cancellationToken = default)
    {
        var holidays = await _repository.GetAllAsync(cancellationToken).ConfigureAwait(false);
        return holidays.Select(holiday => holiday.ToDto()).ToArray();
    }

    /// <inheritdoc />
    public async Task<HolidayDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var holiday = await _repository.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
        return holiday?.ToDto();
    }

    /// <inheritdoc />
    public async Task<HolidayDto> CreateAsync(CreateHolidayRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var entity = request.ToEntity();
        var created = await _repository.AddAsync(entity, cancellationToken).ConfigureAwait(false);
        return created.ToDto();
    }

    /// <inheritdoc />
    public async Task<HolidayDto?> UpdateAsync(Guid id, UpdateHolidayRequest request, CancellationToken cancellationToken = default)
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
