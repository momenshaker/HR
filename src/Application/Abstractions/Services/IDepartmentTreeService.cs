using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HR.Domain.Entities;

namespace HR.Application.Abstractions.Services;

/// <summary>
///     Provides hierarchical operations for departments within an organization.
/// </summary>
public interface IDepartmentTreeService
{
    /// <summary>
    ///     Moves the specified department under a new parent and rebuilds hierarchy metadata for its subtree.
    /// </summary>
    /// <param name="departmentId">The identifier of the department to move.</param>
    /// <param name="newParentDepartmentId">The identifier of the new parent department or <c>null</c> to promote to root.</param>
    /// <param name="cancellationToken">A token for cancelling the operation.</param>
    Task MoveDepartmentAsync(
        Guid departmentId,
        Guid? newParentDepartmentId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Retrieves the specified department and all of its descendants.
    /// </summary>
    /// <param name="rootDepartmentId">The root department of the subtree.</param>
    /// <param name="cancellationToken">A token for cancelling the operation.</param>
    /// <returns>A read-only collection containing the root department followed by its descendants.</returns>
    Task<IReadOnlyCollection<Department>> GetSubtreeAsync(
        Guid rootDepartmentId,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Retrieves the ordered list of ancestor departments for the specified department.
    /// </summary>
    /// <param name="departmentId">The department whose ancestors should be returned.</param>
    /// <param name="cancellationToken">A token for cancelling the operation.</param>
    /// <returns>A read-only collection ordered from the root ancestor down to the immediate parent.</returns>
    Task<IReadOnlyCollection<Department>> GetAncestorsAsync(
        Guid departmentId,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Retrieves the breadcrumb trail (ancestors plus the department itself) for the specified department.
    /// </summary>
    /// <param name="departmentId">The department whose breadcrumb should be returned.</param>
    /// <param name="cancellationToken">A token for cancelling the operation.</param>
    /// <returns>A read-only collection ordered from the root ancestor through the department.</returns>
    Task<IReadOnlyCollection<Department>> GetBreadcrumbAsync(
        Guid departmentId,
        CancellationToken cancellationToken = default);
}
