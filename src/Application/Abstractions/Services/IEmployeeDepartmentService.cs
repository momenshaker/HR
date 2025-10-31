using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace HR.Application.Abstractions.Services;

/// <summary>
///     Service responsible for managing employee to department assignments.
/// </summary>
public interface IEmployeeDepartmentService
{
    /// <summary>
    ///     Assigns the specified employee to the provided departments, adding any missing associations.
    /// </summary>
    Task AssignAsync(
        Guid employeeId,
        IReadOnlyCollection<Guid> departmentIds,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Replaces the employee's department assignments with the provided collection.
    /// </summary>
    Task ReplaceAsync(
        Guid employeeId,
        IReadOnlyCollection<Guid> departmentIds,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Removes the provided department assignments from the employee.
    /// </summary>
    Task UnassignAsync(
        Guid employeeId,
        IReadOnlyCollection<Guid> departmentIds,
        CancellationToken cancellationToken = default);
}
