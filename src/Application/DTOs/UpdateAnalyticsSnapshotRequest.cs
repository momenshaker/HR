using System.ComponentModel.DataAnnotations;

namespace HR.Application.DTOs;

/// <summary>
///     Incoming request payload for updating an analytics snapshot.
/// </summary>
public sealed class UpdateAnalyticsSnapshotRequest
{
    [Required]
    public DateTime CapturedAtUtc { get; init; }

    [Range(0, int.MaxValue)]
    public int Headcount { get; init; }

    [Range(0, 100)]
    public decimal TurnoverRate { get; init; }

    [Range(0, 1000)]
    public decimal AverageTenureMonths { get; init; }

    [Range(0, 1000)]
    public decimal HiringVelocity { get; init; }

    [Range(0, 100)]
    public decimal EngagementScore { get; init; }

    [MaxLength(2000)]
    public string Commentary { get; init; } = string.Empty;
}
