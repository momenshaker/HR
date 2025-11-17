using HR.Application.Abstractions.Repositories;
using HR.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HR.Infrastructure.Persistence.EntityFramework.Repositories;

internal sealed class EntityFrameworkAttendancePunchConfigurationRepository :
    EntityFrameworkRepository<AttendancePunchConfiguration>,
    IAttendancePunchConfigurationRepository
{
    public EntityFrameworkAttendancePunchConfigurationRepository(HrDbContext dbContext)
        : base(dbContext, configuration => configuration.Id)
    {
    }

    public async Task<IReadOnlyCollection<AttendancePunchConfiguration>> GetActiveAsync(CancellationToken cancellationToken = default)
    {
        var configurations = await DbSet
            .AsNoTracking()
            .Where(configuration => configuration.IsActive)
            .OrderBy(configuration => configuration.SortOrder)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        return configurations;
    }

    public Task<AttendancePunchConfiguration?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return GetByIdInternalAsync(id, cancellationToken);
    }

    public Task<AttendancePunchConfiguration> AddAsync(AttendancePunchConfiguration entity, CancellationToken cancellationToken = default)
    {
        return AddInternalAsync(entity, cancellationToken);
    }

    public Task<AttendancePunchConfiguration?> UpdateAsync(AttendancePunchConfiguration entity, CancellationToken cancellationToken = default)
    {
        return UpdateInternalAsync(entity, cancellationToken);
    }
}
