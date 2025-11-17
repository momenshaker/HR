using System;
using System.ComponentModel.DataAnnotations;
using HR.Application.Validation;

namespace HR.Application.DTOs;

/// <summary>
///     Incoming payload for creating a job application.
/// </summary>
public sealed class CreateJobApplicationRequest : IValidatableRequest
{
    [Required]
    public Guid CandidateId { get; init; }

    [Required]
    public Guid JobPostingId { get; init; }

    public DateTime AppliedDate { get; init; } = DateTime.UtcNow;

    [MaxLength(100)]
    public string CurrentStage { get; init; } = string.Empty;

    [MaxLength(50)]
    public string Status { get; init; } = string.Empty;

    [MaxLength(100)]
    public string Source { get; init; } = string.Empty;

    [Url]
    public string CVUrl { get; init; } = string.Empty;

    [MaxLength(4000)]
    public string CoverLetter { get; init; } = string.Empty;

    public decimal? ExpectedSalary { get; init; }

    [MaxLength(100)]
    public string NoticePeriod { get; init; } = string.Empty;
}
