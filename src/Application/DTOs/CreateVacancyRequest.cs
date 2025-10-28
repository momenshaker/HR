using System.ComponentModel.DataAnnotations;

namespace HR.Application.DTOs;

/// <summary>
///     Incoming payload for publishing a new vacancy.
/// </summary>
public sealed class CreateVacancyRequest
{
    [Required]
    [MaxLength(200)]
    public string Title { get; init; } = string.Empty;

    [Required]
    [MaxLength(150)]
    public string Department { get; init; } = string.Empty;

    [Required]
    [MaxLength(150)]
    public string Location { get; init; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string EmploymentType { get; init; } = string.Empty;

    [Required]
    [MaxLength(4000)]
    public string Description { get; init; } = string.Empty;

    public IReadOnlyCollection<string> Responsibilities { get; init; } = Array.Empty<string>();

    public IReadOnlyCollection<string> Requirements { get; init; } = Array.Empty<string>();

    public IReadOnlyCollection<string> PipelineStages { get; init; } = Array.Empty<string>();

    public IReadOnlyCollection<string> HiringTeam { get; init; } = Array.Empty<string>();

    [Url]
    [MaxLength(500)]
    public string ApplicationUrl { get; init; } = string.Empty;

    public DateTime? ClosingAtUtc { get; init; }
}
