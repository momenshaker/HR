namespace HR.Application.DTOs;

/// <summary>
///     Read model representing feedback captured during a review cycle.
/// </summary>
public sealed record PerformanceFeedbackDto(
    Guid Id,
    string FeedbackType,
    string Comments,
    Guid SubmittedBy,
    DateTime SubmittedAtUtc);
