namespace HR.Application.DTOs;

/// <summary>
///     Read model representing a cascaded goal for a performance review.
/// </summary>
public sealed record PerformanceGoalDto(
    Guid Id,
    string Title,
    string Description,
    decimal Weight,
    Guid? ParentGoalId,
    string Alignment,
    string Status);
