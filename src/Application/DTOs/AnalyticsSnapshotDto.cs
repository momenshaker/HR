namespace HR.Application.DTOs;

/// <summary>
///     Read model representing an analytics snapshot.
/// </summary>
public sealed record AnalyticsSnapshotDto(
    Guid Id,
    DateTime CapturedAtUtc,
    int Headcount,
    decimal TurnoverRate,
    decimal AverageTenureMonths,
    decimal HiringVelocity,
    decimal EngagementScore,
    string Commentary);
