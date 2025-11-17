using System;
using System.ComponentModel.DataAnnotations;
using HR.Application.Validation;

namespace HR.Application.DTOs;

/// <summary>
///     Incoming payload for updating an existing vacancy.
/// </summary>
public sealed class UpdateVacancyRequest : IValidatableRequest
{
    [Required]
    public Guid RequisitionId { get; init; }

    [Required]
    [MaxLength(200)]
    public string PublicTitle { get; init; } = string.Empty;

    [Required]
    [MaxLength(150)]
    public string Department { get; init; } = string.Empty;

    [Required]
    [MaxLength(150)]
    public string Location { get; init; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string EmploymentType { get; init; } = string.Empty;

    [MaxLength(50)]
    public string WorkMode { get; init; } = string.Empty;

    public bool SalaryVisible { get; init; }

    [MaxLength(200)]
    public string SalaryRangeText { get; init; } = string.Empty;

    [Range(1, 1000)]
    public int NumberOfPositions { get; init; } = 1;

    [Required]
    [MaxLength(4000)]
    public string PublicDescription { get; init; } = string.Empty;

    public IReadOnlyCollection<string> Responsibilities { get; init; } = Array.Empty<string>();

    public IReadOnlyCollection<string> Requirements { get; init; } = Array.Empty<string>();

    public IReadOnlyCollection<string> PostingChannels { get; init; } = Array.Empty<string>();

    public IReadOnlyCollection<string> PipelineStages { get; init; } = Array.Empty<string>();

    public IReadOnlyCollection<string> HiringTeam { get; init; } = Array.Empty<string>();

    [Url]
    [MaxLength(500)]
    public string ApplicationUrl { get; init; } = string.Empty;

    [MaxLength(50)]
    public string Status { get; init; } = string.Empty;

    public DateTime? PublishedAtUtc { get; init; }

    public DateTime? ClosedAtUtc { get; init; }
}