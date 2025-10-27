namespace HR.Application.DTOs;

/// <summary>
///     Aggregated department headcount metrics for workforce analytics.
/// </summary>
public sealed record EmployeeDepartmentHeadcountDto(
    Guid DepartmentId,
    string DepartmentName,
    int ActiveEmployees,
    int TotalEmployees);
