namespace HR.Application.DTOs;

/// <summary>
///     Summarises workforce composition and movement trends for leadership dashboards.
/// </summary>
public sealed record EmployeeWorkforceSnapshotDto(
    int TotalEmployees,
    int ActiveEmployees,
    int InactiveEmployees,
    int NewHiresLast30Days,
    int DeparturesLast30Days,
    int UpcomingDeparturesNext30Days,
    double AverageTenureInYears,
    IReadOnlyCollection<EmployeeDepartmentHeadcountDto> DepartmentHeadcounts);
