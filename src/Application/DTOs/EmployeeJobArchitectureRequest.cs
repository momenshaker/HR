using System.ComponentModel.DataAnnotations;

namespace HR.Application.DTOs;

/// <summary>
///     Request payload capturing job architecture metadata for an employee.
/// </summary>
public sealed class EmployeeJobArchitectureRequest
{
    [MaxLength(100)]
    public string JobFamily { get; init; } = string.Empty;

    [MaxLength(100)]
    public string JobFunction { get; init; } = string.Empty;

    [MaxLength(50)]
    public string JobLevel { get; init; } = string.Empty;

    [MaxLength(50)]
    public string JobCode { get; init; } = string.Empty;

    [MaxLength(100)]
    public string CareerTrack { get; init; } = string.Empty;
}
