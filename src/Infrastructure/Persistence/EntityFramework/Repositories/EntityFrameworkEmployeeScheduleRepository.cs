using HR.Application.Abstractions.Repositories;
using HR.Domain.Entities;

namespace HR.Infrastructure.Persistence.EntityFramework.Repositories;

internal sealed class EntityFrameworkEmployeeScheduleRepository : EntityFrameworkRepository<EmployeeSchedule>, IEmployeeScheduleRepository
{
    public EntityFrameworkEmployeeScheduleRepository(HrDbContext dbContext)
        : base(dbContext, schedule => schedule.Id)
    {
    }

    public async Task<IReadOnlyCollection<EmployeeSchedule>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await GetAllInternalAsync(cancellationToken).ConfigureAwait(false);
    }

    public Task<EmployeeSchedule?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return GetByIdInternalAsync(id, cancellationToken);
    }

    public Task<EmployeeSchedule> AddAsync(EmployeeSchedule schedule, CancellationToken cancellationToken = default)
    {
        return AddInternalAsync(schedule, cancellationToken);
    }

    public Task<EmployeeSchedule?> UpdateAsync(EmployeeSchedule schedule, CancellationToken cancellationToken = default)
    {
        return UpdateInternalAsync(schedule, cancellationToken);
    }

    public Task<bool> RemoveAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return RemoveInternalAsync(id, cancellationToken);
    }
}
