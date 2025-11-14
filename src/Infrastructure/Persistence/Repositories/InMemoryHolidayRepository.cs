using System.Collections.Concurrent;
using HR.Application.Abstractions.Repositories;
using HR.Domain.Entities;

namespace HR.Infrastructure.Persistence.Repositories;

internal sealed class InMemoryHolidayRepository : IHolidayRepository
{
    private readonly ConcurrentDictionary<Guid, Holiday> _holidays = new();

    public Task<IReadOnlyCollection<Holiday>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        IReadOnlyCollection<Holiday> snapshot = _holidays.Values.Select(Clone).ToArray();
        return Task.FromResult(snapshot);
    }

    public Task<Holiday?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _holidays.TryGetValue(id, out var holiday);
        return Task.FromResult(holiday is null ? null : Clone(holiday));
    }

    public Task<Holiday> AddAsync(Holiday holiday, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(holiday);
        _holidays[holiday.Id] = Clone(holiday);
        return Task.FromResult(holiday);
    }

    public Task<Holiday?> UpdateAsync(Holiday holiday, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(holiday);

        if (!_holidays.ContainsKey(holiday.Id))
        {
            return Task.FromResult<Holiday?>(null);
        }

        _holidays[holiday.Id] = Clone(holiday);
        return Task.FromResult<Holiday?>(holiday);
    }

    public Task<bool> RemoveAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_holidays.TryRemove(id, out _));
    }

    private static Holiday Clone(Holiday holiday)
    {
        return new Holiday
        {
            Id = holiday.Id,
            OrganizationId = holiday.OrganizationId,
            Date = holiday.Date,
            Name = holiday.Name,
            IsPaid = holiday.IsPaid,
            CountryCode = holiday.CountryCode,
            Description = holiday.Description
        };
    }
}
