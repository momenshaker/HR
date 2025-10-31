using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HR.Application.Abstractions.Repositories;
using HR.Domain.Entities;
using Microsoft.EntityFrameworkCore;

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

    public async Task<IReadOnlyCollection<Department>> GetByIdsAsync(
        IReadOnlyCollection<Guid> departmentIds,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(departmentIds);

        if (departmentIds.Count == 0)
        {
            return Array.Empty<Department>();
        }

        return await DbContext.Departments
            .Where(department => departmentIds.Contains(department.Id))
            .AsNoTracking()
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
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

    public async Task<bool> ExistsByNameAsync(
        Guid organizationId,
        Guid? parentDepartmentId,
        string name,
        Guid? excludingDepartmentId = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        var normalizedName = name.Trim().ToUpperInvariant();
        var query = DbContext.Departments
            .AsNoTracking()
            .Where(department => department.OrganizationId == organizationId)
            .Where(department => department.Name.ToUpper() == normalizedName);

        if (parentDepartmentId.HasValue)
        {
            query = query.Where(department => department.ParentDepartmentId == parentDepartmentId.Value);
        }
        else
        {
            query = query.Where(department => department.ParentDepartmentId == null);
        }

        if (excludingDepartmentId.HasValue)
        {
            query = query.Where(department => department.Id != excludingDepartmentId.Value);
        }

        return await query.AnyAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task<bool> ExistsByCodeAsync(
        Guid organizationId,
        string code,
        Guid? excludingDepartmentId = null,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            return false;
        }

        var normalizedCode = code.Trim().ToUpperInvariant();
        var query = DbContext.Departments
            .AsNoTracking()
            .Where(department => department.OrganizationId == organizationId)
            .Where(department => department.Code == normalizedCode);

        if (excludingDepartmentId.HasValue)
        {
            query = query.Where(department => department.Id != excludingDepartmentId.Value);
        }

        return await query.AnyAsync(cancellationToken).ConfigureAwait(false);
    }
}
