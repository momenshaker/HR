using HR.Application.DTOs;

namespace HR.Application.Abstractions.Services;

/// <summary>
///     Extension methods that expose advanced employee search functionality while preserving interface segregation.
/// </summary>
public static class EmployeeServiceSearchExtensions
{
    /// <summary>
    ///     Performs an advanced employee search using the underlying implementation when available.
    /// </summary>
    /// <param name="employeeService">The employee service instance.</param>
    /// <param name="request">The search request describing filters, sorting, and pagination rules.</param>
    /// <param name="cancellationToken">Token used to observe cancellation signals.</param>
    /// <returns>A paginated collection of employees that match the provided criteria.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="employeeService" /> or <paramref name="request" /> is null.</exception>
    /// <exception cref="NotSupportedException">Thrown when the supplied service does not support search operations.</exception>
    public static Task<PaginatedResponse<EmployeeDto>> SearchAsync(
        this IEmployeeService employeeService,
        EmployeeSearchRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(employeeService);
        ArgumentNullException.ThrowIfNull(request);

        if (employeeService is IEmployeeSearchService searchableService)
        {
            return searchableService.SearchAsync(request, cancellationToken);
        }

        throw new NotSupportedException("The configured employee service does not support search operations.");
    }
}
