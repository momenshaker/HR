namespace HR.Application.DTOs;

/// <summary>
///     Read model representing a published vacancy.
/// </summary>
public sealed record VacancyDto(
    Guid Id,
    string Title,
    string Department,
    string Location,
    string EmploymentType,
    string Description,
    IReadOnlyCollection<string> Responsibilities,
    IReadOnlyCollection<string> Requirements,
    IReadOnlyCollection<string> PipelineStages,
    IReadOnlyCollection<string> HiringTeam,
    DateTime PostedAtUtc,
    DateTime? ClosingAtUtc,
    string Status,
    string ApplicationUrl);
