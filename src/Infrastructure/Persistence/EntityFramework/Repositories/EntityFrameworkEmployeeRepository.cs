using HR.Application.Abstractions.Repositories;
using HR.Domain.Entities;

namespace HR.Infrastructure.Persistence.EntityFramework.Repositories;

internal sealed class EntityFrameworkEmployeeRepository : EntityFrameworkRepository<Employee>, IEmployeeRepository
{
    public EntityFrameworkEmployeeRepository(HrDbContext dbContext)
        : base(dbContext, employee => employee.Id)
    {
    }

    public async Task<IReadOnlyCollection<Employee>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await GetAllInternalAsync(cancellationToken).ConfigureAwait(false);
    }

    public Task<Employee?> GetByIdAsync(Guid employeeId, CancellationToken cancellationToken = default)
    {
        return GetByIdInternalAsync(employeeId, cancellationToken);
    }

    public Task<Employee> AddAsync(Employee employee, CancellationToken cancellationToken = default)
    {
        return AddInternalAsync(employee, cancellationToken);
    }

    public Task<Employee?> UpdateAsync(Employee employee, CancellationToken cancellationToken = default)
    {
        return UpdateInternalAsync(employee, cancellationToken);
    }

    public Task<bool> RemoveAsync(Guid employeeId, CancellationToken cancellationToken = default)
    {
        return RemoveInternalAsync(employeeId, cancellationToken);
    }
}
