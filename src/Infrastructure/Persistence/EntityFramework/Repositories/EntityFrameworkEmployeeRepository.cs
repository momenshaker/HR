using HR.Application.Abstractions.Repositories;
using HR.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HR.Infrastructure.Persistence.EntityFramework.Repositories;

internal sealed class EntityFrameworkEmployeeRepository : EntityFrameworkRepository<Employee>, IEmployeeRepository
{
    public EntityFrameworkEmployeeRepository(HrDbContext dbContext)
        : base(dbContext, employee => employee.Id)
    {
    }

    public async Task<IReadOnlyCollection<Employee>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await DbContext.Employees
            .AsNoTracking()
            .Include(employee => employee.Departments)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<Employee?> GetByIdAsync(Guid employeeId, CancellationToken cancellationToken = default)
    {
        var employee = await DbContext.Employees
            .AsNoTracking()
            .Include(entity => entity.Departments)
            .FirstOrDefaultAsync(entity => entity.Id == employeeId, cancellationToken)
            .ConfigureAwait(false);

        return employee;
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
