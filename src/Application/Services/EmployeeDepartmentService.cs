using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using HR.Application.Abstractions.Repositories;
using HR.Application.Abstractions.Services;

namespace HR.Application.Services;

/// <inheritdoc />
public sealed class EmployeeDepartmentService : IEmployeeDepartmentService
{
    private readonly IDepartmentRepository _departmentRepository;
    private readonly IEmployeeDepartmentRepository _employeeDepartmentRepository;

    public EmployeeDepartmentService(
        IEmployeeDepartmentRepository employeeDepartmentRepository,
        IDepartmentRepository departmentRepository)
    {
        _employeeDepartmentRepository = employeeDepartmentRepository
            ?? throw new ArgumentNullException(nameof(employeeDepartmentRepository));
        _departmentRepository = departmentRepository ?? throw new ArgumentNullException(nameof(departmentRepository));
    }

    /// <inheritdoc />
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

        var uniqueDepartmentIds = departmentIds.Distinct().ToArray();
        EnsureDepartmentIdentifiersAreValid(uniqueDepartmentIds);

        var existingDepartmentIds = (await _employeeDepartmentRepository
                .GetDepartmentIdsByEmployeeAsync(employeeId, cancellationToken)
                .ConfigureAwait(false))
            .ToHashSet();

        var combinedDepartmentIds = existingDepartmentIds.Union(uniqueDepartmentIds).ToArray();
        await ValidateDepartmentsAsync(combinedDepartmentIds, cancellationToken).ConfigureAwait(false);

        var departmentIdsToAssign = uniqueDepartmentIds
            .Where(departmentId => !existingDepartmentIds.Contains(departmentId))
            .ToArray();

        if (departmentIdsToAssign.Length == 0)
        {
            return;
        }

        await _employeeDepartmentRepository
            .AssignAsync(employeeId, departmentIdsToAssign, cancellationToken)
            .ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task ReplaceAsync(
        Guid employeeId,
        IReadOnlyCollection<Guid> departmentIds,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(departmentIds);

        if (departmentIds.Count == 0)
        {
            await _employeeDepartmentRepository
                .ReplaceAsync(employeeId, Array.Empty<Guid>(), cancellationToken)
                .ConfigureAwait(false);
            return;
        }

        var uniqueDepartmentIds = departmentIds.Distinct().ToArray();
        EnsureDepartmentIdentifiersAreValid(uniqueDepartmentIds);

        await ValidateDepartmentsAsync(uniqueDepartmentIds, cancellationToken).ConfigureAwait(false);

        await _employeeDepartmentRepository
            .ReplaceAsync(employeeId, uniqueDepartmentIds, cancellationToken)
            .ConfigureAwait(false);
    }

    /// <inheritdoc />
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

        var uniqueDepartmentIds = departmentIds.Distinct().ToArray();
        EnsureDepartmentIdentifiersAreValid(uniqueDepartmentIds);

        await _employeeDepartmentRepository
            .UnassignAsync(employeeId, uniqueDepartmentIds, cancellationToken)
            .ConfigureAwait(false);
    }

    private async Task ValidateDepartmentsAsync(
        IReadOnlyCollection<Guid> departmentIds,
        CancellationToken cancellationToken)
    {
        if (departmentIds.Count == 0)
        {
            return;
        }

        var departments = await _departmentRepository
            .GetByIdsAsync(departmentIds, cancellationToken)
            .ConfigureAwait(false);

        var expectedCount = departmentIds.Count;
        if (departments.Count != expectedCount)
        {
            var foundIdentifiers = departments.Select(department => department.Id).ToHashSet();
            var missingIdentifiers = departmentIds.Where(id => !foundIdentifiers.Contains(id)).ToArray();
            throw new ValidationException(
                $"The following departments could not be found: {string.Join(", ", missingIdentifiers)}");
        }

        var organizationCount = departments.Select(department => department.OrganizationId).Distinct().Count();
        if (organizationCount > 1)
        {
            throw new ValidationException("Departments must belong to a single organization.");
        }
    }

    private static void EnsureDepartmentIdentifiersAreValid(IReadOnlyCollection<Guid> departmentIds)
    {
        if (departmentIds.Count == 0)
        {
            return;
        }

        if (departmentIds.Any(departmentId => departmentId == Guid.Empty))
        {
            throw new ValidationException("Department identifiers must be specified.");
        }
    }
}
