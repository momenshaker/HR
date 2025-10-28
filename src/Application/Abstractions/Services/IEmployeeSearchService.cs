using HR.Application.DTOs;

namespace HR.Application.Abstractions.Services;

/// <summary>
///     Provides advanced employee search capabilities with filtering, sorting, and paging semantics.
/// </summary>
public interface IEmployeeSearchService
{
    /// <summary>
    ///     Performs an advanced employee search using filtering, sorting, and paging semantics.
    /// </summary>
    /// <param name="request">The search request describing filters, sorting, and pagination rules.</param>
    /// <param name="cancellationToken">Token used to observe cancellation signals.</param>
    /// <returns>A paginated collection of employees that match the provided criteria.</returns>
    Task<PaginatedResponse<EmployeeDto>> SearchAsync(
        EmployeeSearchRequest request,
        CancellationToken cancellationToken = default);
}
