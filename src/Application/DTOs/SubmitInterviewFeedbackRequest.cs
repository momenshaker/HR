using System;
using System.ComponentModel.DataAnnotations;
using HR.Application.Validation;

namespace HR.Application.DTOs;

/// <summary>
///     Incoming payload for capturing interviewer feedback.
/// </summary>
public sealed class SubmitInterviewFeedbackRequest : IValidatableRequest
{
    public Guid? InterviewId { get; init; }

    public Guid? ApplicationId { get; init; }

    public Guid? StageId { get; init; }

    [Required]
    public Guid ReviewerId { get; init; }

    [Range(1, 5)]
    public int RatingOverall { get; init; }

    [Range(1, 5)]
    public int? RatingTechnical { get; init; }

    [Range(1, 5)]
    public int? RatingCultureFit { get; init; }

    [MaxLength(2000)]
    public string Strengths { get; init; } = string.Empty;

    [MaxLength(2000)]
    public string Weaknesses { get; init; } = string.Empty;

    [MaxLength(50)]
    public string Recommendation { get; init; } = string.Empty;
}
