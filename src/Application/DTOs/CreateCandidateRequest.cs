using System.ComponentModel.DataAnnotations;

namespace HR.Application.DTOs;

/// <summary>
///     Incoming request payload for creating a candidate record.
/// </summary>
public sealed class CreateCandidateRequest
{
    [Required]
    [MaxLength(150)]
    public string FullName { get; init; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; init; } = string.Empty;

    [Required]
    [MaxLength(150)]
    public string AppliedRole { get; init; } = string.Empty;

    [MaxLength(100)]
    public string Stage { get; init; } = string.Empty;

    [MaxLength(100)]
    public string Source { get; init; } = string.Empty;

    [Url]
    public string ResumeUrl { get; init; } = string.Empty;

    [MaxLength(2000)]
    public string Notes { get; init; } = string.Empty;

    public DateTime? NextInterviewAtUtc { get; init; }
}
