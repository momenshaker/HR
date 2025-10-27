using HR.Application.Abstractions.Repositories;
using HR.Domain.Entities;

namespace HR.Infrastructure.Persistence.EntityFramework.Repositories;

internal sealed class EntityFrameworkDepartmentRepository : EntityFrameworkRepository<Department>, IDepartmentRepository
{
    public EntityFrameworkDepartmentRepository(HrDbContext dbContext)
        : base(dbContext, department => department.Id)
    {
    }

    public async Task<IReadOnlyCollection<Department>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await GetAllInternalAsync(cancellationToken).ConfigureAwait(false);
    }

    public Task<Department?> GetByIdAsync(Guid departmentId, CancellationToken cancellationToken = default)
    {
        return GetByIdInternalAsync(departmentId, cancellationToken);
    }

    public Task<Department> AddAsync(Department department, CancellationToken cancellationToken = default)
    {
        return AddInternalAsync(department, cancellationToken);
    }

    public Task<Department?> UpdateAsync(Department department, CancellationToken cancellationToken = default)
    {
        return UpdateInternalAsync(department, cancellationToken);
    }

    public Task<bool> RemoveAsync(Guid departmentId, CancellationToken cancellationToken = default)
    {
        return RemoveInternalAsync(departmentId, cancellationToken);
    }
}
