using System;

namespace HR.Application.DTOs;

/// <summary>
///     Read model for interviewer feedback and evaluation.
/// </summary>
public sealed record InterviewFeedbackDto(
    Guid Id,
    Guid? InterviewId,
    Guid? ApplicationId,
    Guid? StageId,
    Guid ReviewerId,
    int RatingOverall,
    int? RatingTechnical,
    int? RatingCultureFit,
    string Strengths,
    string Weaknesses,
    string Recommendation,
    DateTime CreatedAt);
