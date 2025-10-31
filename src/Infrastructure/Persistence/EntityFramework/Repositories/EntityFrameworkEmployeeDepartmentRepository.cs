using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HR.Application.Abstractions.Repositories;
using HR.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HR.Infrastructure.Persistence.EntityFramework.Repositories;

internal sealed class EntityFrameworkEmployeeDepartmentRepository : IEmployeeDepartmentRepository
{
    public EntityFrameworkEmployeeDepartmentRepository(HrDbContext dbContext)
    {
        DbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    private HrDbContext DbContext { get; }

    public async Task<IReadOnlyCollection<Guid>> GetDepartmentIdsByEmployeeAsync(
        Guid employeeId,
        CancellationToken cancellationToken = default)
    {
        return await DbContext.EmployeeDepartments
            .Where(assignment => assignment.EmployeeId == employeeId)
            .Select(assignment => assignment.DepartmentId)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task AssignAsync(
        Guid employeeId,
        IReadOnlyCollection<Guid> departmentIds,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(departmentIds);

        if (departmentIds.Count == 0)
        {
            return;
        }

        var targetDepartmentIds = departmentIds.ToArray();

        var existingAssignments = await DbContext.EmployeeDepartments
            .Where(assignment => assignment.EmployeeId == employeeId && targetDepartmentIds.Contains(assignment.DepartmentId))
            .Select(assignment => assignment.DepartmentId)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        var existingSet = existingAssignments.ToHashSet();
        var assignmentsToAdd = targetDepartmentIds
            .Where(departmentId => !existingSet.Contains(departmentId))
            .Select(departmentId => new EmployeeDepartment
            {
                EmployeeId = employeeId,
                DepartmentId = departmentId,
                IsPrimary = false
            })
            .ToArray();

        if (assignmentsToAdd.Length == 0)
        {
            return;
        }

        await DbContext.EmployeeDepartments.AddRangeAsync(assignmentsToAdd, cancellationToken).ConfigureAwait(false);
        await DbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task ReplaceAsync(
        Guid employeeId,
        IReadOnlyCollection<Guid> departmentIds,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(departmentIds);

        var desiredDepartmentIds = departmentIds.ToHashSet();

        await using var transaction = await DbContext.Database
            .BeginTransactionAsync(cancellationToken)
            .ConfigureAwait(false);

        var existingAssignments = await DbContext.EmployeeDepartments
            .Where(assignment => assignment.EmployeeId == employeeId)
            .AsTracking()
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        var existingIds = existingAssignments.Select(assignment => assignment.DepartmentId).ToHashSet();

        var assignmentsToRemove = existingAssignments
            .Where(assignment => !desiredDepartmentIds.Contains(assignment.DepartmentId))
            .ToArray();

        if (assignmentsToRemove.Length > 0)
        {
            DbContext.EmployeeDepartments.RemoveRange(assignmentsToRemove);
        }

        var assignmentsToAdd = desiredDepartmentIds
            .Except(existingIds)
            .Select(departmentId => new EmployeeDepartment
            {
                EmployeeId = employeeId,
                DepartmentId = departmentId,
                IsPrimary = false
            })
            .ToArray();

        if (assignmentsToAdd.Length > 0)
        {
            await DbContext.EmployeeDepartments.AddRangeAsync(assignmentsToAdd, cancellationToken).ConfigureAwait(false);
        }

        if (assignmentsToRemove.Length == 0 && assignmentsToAdd.Length == 0)
        {
            await transaction.CommitAsync(cancellationToken).ConfigureAwait(false);
            return;
        }

        await DbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        await transaction.CommitAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task UnassignAsync(
        Guid employeeId,
        IReadOnlyCollection<Guid> departmentIds,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(departmentIds);

        if (departmentIds.Count == 0)
        {
            return;
        }

        var targetDepartmentIds = departmentIds.ToArray();

        var assignmentsToRemove = await DbContext.EmployeeDepartments
            .Where(assignment => assignment.EmployeeId == employeeId && targetDepartmentIds.Contains(assignment.DepartmentId))
            .AsTracking()
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        if (assignmentsToRemove.Count == 0)
        {
            return;
        }

        DbContext.EmployeeDepartments.RemoveRange(assignmentsToRemove);
        await DbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }
}
