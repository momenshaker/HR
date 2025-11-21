using HR.Application.DTOs;

namespace HR.Application.Abstractions.Services;

/// <summary>
///     Hierarchy operations for employees.
/// </summary>
public partial interface IEmployeeService
{
    /// <summary>
    ///     Builds an employee hierarchy using reporting relationships between positions.
    /// </summary>
    /// <remarks>
    ///     Only positions that are occupied by an employee are included in the hierarchy.
    /// </remarks>
    Task<IReadOnlyCollection<EmployeeHierarchyNodeDto>> GetHierarchyAsync(
        CancellationToken cancellationToken = default);
}
