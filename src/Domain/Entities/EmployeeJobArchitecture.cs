namespace HR.Domain.Entities;

/// <summary>
///     Represents the job architecture profile assigned to an employee.
/// </summary>
public sealed class EmployeeJobArchitecture
{
    /// <summary>
    ///     Gets a reusable empty job architecture instance.
    /// </summary>
    public static EmployeeJobArchitecture Empty { get; } = new();

    /// <summary>
    ///     Gets the job family grouping.
    /// </summary>
    public string JobFamily { get; init; } = string.Empty;

    /// <summary>
    ///     Gets the job function or discipline for the role.
    /// </summary>
    public string JobFunction { get; init; } = string.Empty;

    /// <summary>
    ///     Gets the job level, grade, or band.
    /// </summary>
    public string JobLevel { get; init; } = string.Empty;

    /// <summary>
    ///     Gets the job architecture code used for downstream systems.
    /// </summary>
    public string JobCode { get; init; } = string.Empty;

    /// <summary>
    ///     Gets the career track or progression grouping (e.g. Individual Contributor, Manager).
    /// </summary>
    public string CareerTrack { get; init; } = string.Empty;
}
