using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HR.Domain.Entities;

namespace HR.Application.Abstractions.Repositories;

/// <summary>
///     Repository abstraction for managing <see cref="EmployeeDepartment" /> assignments.
/// </summary>
public interface IEmployeeDepartmentRepository
{
    /// <summary>
    ///     Retrieves the identifiers of departments associated with the specified employee.
    /// </summary>
    Task<IReadOnlyCollection<Guid>> GetDepartmentIdsByEmployeeAsync(
        Guid employeeId,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Adds department assignments for the provided employee.
    /// </summary>
    Task AssignAsync(
        Guid employeeId,
        IReadOnlyCollection<Guid> departmentIds,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Replaces the current department assignments for the provided employee.
    /// </summary>
    Task ReplaceAsync(
        Guid employeeId,
        IReadOnlyCollection<Guid> departmentIds,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Removes department assignments for the provided employee.
    /// </summary>
    Task UnassignAsync(
        Guid employeeId,
        IReadOnlyCollection<Guid> departmentIds,
        CancellationToken cancellationToken = default);
}
