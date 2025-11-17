using System;
using System.ComponentModel.DataAnnotations;
using HR.Application.Validation;

namespace HR.Application.DTOs;

/// <summary>
///     Incoming request payload for creating a candidate record.
/// </summary>
public sealed class CreateCandidateRequest : IValidatableRequest
{
    [Required]
    [MaxLength(150)]
    public string FullName { get; init; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; init; } = string.Empty;

    [Phone]
    [MaxLength(50)]
    public string Phone { get; init; } = string.Empty;

    [MaxLength(150)]
    public string CurrentCompany { get; init; } = string.Empty;

    [MaxLength(150)]
    public string CurrentTitle { get; init; } = string.Empty;

    [Range(0, 80)]
    public int? YearsOfExperience { get; init; }

    [Required]
    [MaxLength(150)]
    public string AppliedRole { get; init; } = string.Empty;

    [MaxLength(100)]
    public string Stage { get; init; } = string.Empty;

    [MaxLength(100)]
    public string Source { get; init; } = string.Empty;

    [Url]
    public string LinkedInProfileUrl { get; init; } = string.Empty;

    [Url]
    public string ResumeUrl { get; init; } = string.Empty;

    [MaxLength(2000)]
    public string Notes { get; init; } = string.Empty;

    public IReadOnlyCollection<string> Tags { get; init; } = Array.Empty<string>();

    public bool IsInTalentPool { get; init; }

    public DateTime? NextInterviewAtUtc { get; init; }
}