using HR.Application.Abstractions.Repositories;
using HR.Domain.Entities;

namespace HR.Infrastructure.Persistence.EntityFramework.Repositories;

internal sealed class EntityFrameworkHolidayRepository : EntityFrameworkRepository<Holiday>, IHolidayRepository
{
    public EntityFrameworkHolidayRepository(HrDbContext dbContext)
        : base(dbContext, holiday => holiday.Id)
    {
    }

    public async Task<IReadOnlyCollection<Holiday>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await GetAllInternalAsync(cancellationToken).ConfigureAwait(false);
    }

    public Task<Holiday?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return GetByIdInternalAsync(id, cancellationToken);
    }

    public Task<Holiday> AddAsync(Holiday holiday, CancellationToken cancellationToken = default)
    {
        return AddInternalAsync(holiday, cancellationToken);
    }

    public Task<Holiday?> UpdateAsync(Holiday holiday, CancellationToken cancellationToken = default)
    {
        return UpdateInternalAsync(holiday, cancellationToken);
    }

    public Task<bool> RemoveAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return RemoveInternalAsync(id, cancellationToken);
    }
}
