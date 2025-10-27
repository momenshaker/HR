namespace HR.Application.DTOs;

/// <summary>
///     Supported fields when sorting employees in search results.
/// </summary>
public enum EmployeeSortField
{
    /// <summary>
    ///     Orders employees alphabetically by last name then first name.
    /// </summary>
    LastName = 0,

    /// <summary>
    ///     Orders employees by their employment start date.
    /// </summary>
    EmploymentStartDate = 1,

    /// <summary>
    ///     Orders employees by their employment end date.
    /// </summary>
    EmploymentEndDate = 2,

    /// <summary>
    ///     Orders employees by job title.
    /// </summary>
    JobTitle = 3,

    /// <summary>
    ///     Orders employees by calculated tenure in days.
    /// </summary>
    Tenure = 4
}
