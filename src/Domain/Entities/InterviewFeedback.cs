using System;

namespace HR.Domain.Entities;

/// <summary>
///     Captures evaluation data provided by interviewers.
/// </summary>
public sealed class InterviewFeedback
{
    public Guid Id { get; init; }

    public Guid? InterviewId { get; init; }

    public Guid? ApplicationId { get; init; }

    public Guid? StageId { get; init; }

    public Guid ReviewerId { get; init; }

    public int RatingOverall { get; init; }

    public int? RatingTechnical { get; init; }

    public int? RatingCultureFit { get; init; }

    public string Strengths { get; init; } = string.Empty;

    public string Weaknesses { get; init; } = string.Empty;

    public string Recommendation { get; init; } = string.Empty;

    public DateTime CreatedAt { get; init; }
}
