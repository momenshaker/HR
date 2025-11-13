using System.Linq;
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
            .ThenInclude(membership => membership.Department)
            .Include(employee => employee.ProfileDocuments)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<Employee?> GetByIdAsync(Guid employeeId, CancellationToken cancellationToken = default)
    {
        var employee = await DbContext.Employees
            .AsNoTracking()
            .Include(entity => entity.Departments)
            .ThenInclude(membership => membership.Department)
            .Include(entity => entity.ProfileDocuments)
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

    public async Task<bool> ExistsByEmailAsync(
        string email,
        Guid? excludingEmployeeId = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(email);

        var normalizedEmail = email.Trim().ToUpperInvariant();
        var query = DbContext.Employees
            .AsNoTracking()
            .Where(employee => employee.Email.ToUpper() == normalizedEmail);

        if (excludingEmployeeId.HasValue)
        {
            query = query.Where(employee => employee.Id != excludingEmployeeId.Value);
        }

        return await query.AnyAsync(cancellationToken).ConfigureAwait(false);
    }
}
