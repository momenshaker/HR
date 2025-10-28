using HR.Application.DTOs;

namespace HR.Application.Abstractions.Services;

/// <summary>
///     Workforce analytics operations that are part of the employee service contract.
/// </summary>
public partial interface IEmployeeService
{
    /// <summary>
    ///     Calculates workforce analytics metrics including headcount, movement, and tenure trends.
    /// </summary>
    Task<EmployeeWorkforceSnapshotDto> GetWorkforceSnapshotAsync(
        CancellationToken cancellationToken = default);
}
