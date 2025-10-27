using HR.Application.DTOs;
using HR.Domain.Entities;

namespace HR.Application.Mappings;

/// <summary>
///     Mapping helpers for <see cref="AnalyticsSnapshot" /> entities.
/// </summary>
public static class AnalyticsSnapshotMappings
{
    public static AnalyticsSnapshotDto ToDto(this AnalyticsSnapshot snapshot)
    {
        ArgumentNullException.ThrowIfNull(snapshot);

        return new AnalyticsSnapshotDto(
            snapshot.Id,
            snapshot.CapturedAtUtc,
            snapshot.Headcount,
            snapshot.TurnoverRate,
            snapshot.AverageTenureMonths,
            snapshot.HiringVelocity,
            snapshot.EngagementScore,
            snapshot.Commentary);
    }

    public static AnalyticsSnapshot ToEntity(this CreateAnalyticsSnapshotRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        return new AnalyticsSnapshot
        {
            Id = Guid.NewGuid(),
            CapturedAtUtc = request.CapturedAtUtc,
            Headcount = request.Headcount,
            TurnoverRate = request.TurnoverRate,
            AverageTenureMonths = request.AverageTenureMonths,
            HiringVelocity = request.HiringVelocity,
            EngagementScore = request.EngagementScore,
            Commentary = request.Commentary.Trim()
        };
    }

    public static AnalyticsSnapshot ApplyUpdates(this UpdateAnalyticsSnapshotRequest request, AnalyticsSnapshot existing)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(existing);

        return new AnalyticsSnapshot
        {
            Id = existing.Id,
            CapturedAtUtc = request.CapturedAtUtc,
            Headcount = request.Headcount,
            TurnoverRate = request.TurnoverRate,
            AverageTenureMonths = request.AverageTenureMonths,
            HiringVelocity = request.HiringVelocity,
            EngagementScore = request.EngagementScore,
            Commentary = request.Commentary.Trim()
        };
    }
}
