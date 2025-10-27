using System.ComponentModel.DataAnnotations;

namespace HR.Application.DTOs;

/// <summary>
///     Represents optional filtering, sorting, and paging rules for querying employees.
/// </summary>
public sealed class EmployeeSearchRequest
{
    private const int DefaultPageNumber = 1;
    private const int DefaultPageSize = 25;

    /// <summary>
    ///     Optional free-text query that is matched against an employee's name, email, or job title.
    /// </summary>
    [MaxLength(200)]
    public string? Query { get; init; }

    /// <summary>
    ///     Restricts results to a specific department.
    /// </summary>
    public Guid? DepartmentId { get; init; }

    /// <summary>
    ///     Restricts results to a particular job title.
    /// </summary>
    [MaxLength(150)]
    public string? JobTitle { get; init; }

    /// <summary>
    ///     Indicates whether the employee should currently be active (true) or inactive (false).
    /// </summary>
    public bool? IsActive { get; init; }

    /// <summary>
    ///     Lower bound for an employee's employment start date.
    /// </summary>
    public DateOnly? HiredFrom { get; init; }

    /// <summary>
    ///     Upper bound for an employee's employment start date.
    /// </summary>
    public DateOnly? HiredTo { get; init; }

    /// <summary>
    ///     Lower bound for an employee's employment end date.
    /// </summary>
    public DateOnly? EmploymentEndFrom { get; init; }

    /// <summary>
    ///     Upper bound for an employee's employment end date.
    /// </summary>
    public DateOnly? EmploymentEndTo { get; init; }

    /// <summary>
    ///     1-based index of the page to retrieve.
    /// </summary>
    [Range(1, 1000)]
    public int PageNumber { get; init; } = DefaultPageNumber;

    /// <summary>
    ///     Maximum number of records to include in a single page.
    /// </summary>
    [Range(1, 200)]
    public int PageSize { get; init; } = DefaultPageSize;

    /// <summary>
    ///     Primary field that should be used for ordering the result set.
    /// </summary>
    public EmployeeSortField SortBy { get; init; } = EmployeeSortField.LastName;

    /// <summary>
    ///     Direction to apply when ordering results.
    /// </summary>
    public SortDirection SortDirection { get; init; } = SortDirection.Ascending;
}
