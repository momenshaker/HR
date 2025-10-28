namespace HR.Application.DTOs;

/// <summary>
///     Transport model describing the job architecture metadata for an employee.
/// </summary>
public sealed record EmployeeJobArchitectureDto(
    string JobFamily,
    string JobFunction,
    string JobLevel,
    string JobCode,
    string CareerTrack);
