using HR.Application.Abstractions.Repositories;
using HR.Domain.Entities;
using HR.Infrastructure.Persistence.EntityFramework.Seeders;

namespace HR.Infrastructure.Persistence.Repositories;

public sealed class InMemoryAttendancePunchConfigurationRepository : IAttendancePunchConfigurationRepository
{
    private readonly List<AttendancePunchConfiguration> _store = AttendancePunchConfigurationSeeder.GetSeedData().ToList();

    public Task<IReadOnlyCollection<AttendancePunchConfiguration>> GetActiveAsync(CancellationToken cancellationToken = default)
    {
        var active = _store.Where(configuration => configuration.IsActive).OrderBy(configuration => configuration.SortOrder).ToArray();
        return Task.FromResult<IReadOnlyCollection<AttendancePunchConfiguration>>(active);
    }

    public Task<AttendancePunchConfiguration?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var configuration = _store.FirstOrDefault(cfg => cfg.Id == id);
        return Task.FromResult(configuration);
    }

    public Task<AttendancePunchConfiguration> AddAsync(AttendancePunchConfiguration entity, CancellationToken cancellationToken = default)
    {
        _store.Add(entity);
        return Task.FromResult(entity);
    }

    public Task<AttendancePunchConfiguration?> UpdateAsync(AttendancePunchConfiguration entity, CancellationToken cancellationToken = default)
    {
        var index = _store.FindIndex(cfg => cfg.Id == entity.Id);
        if (index < 0)
        {
            return Task.FromResult<AttendancePunchConfiguration?>(null);
        }

        _store[index] = entity;
        return Task.FromResult<AttendancePunchConfiguration?>(entity);
    }
}
