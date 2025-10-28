namespace HR.Application.DTOs;

/// <summary>
///     Data transfer object for engagement campaign information.
/// </summary>
public sealed record EngagementCampaignDto(
    Guid Id,
    string Name,
    string Description,
    string Channels,
    string TargetAudience,
    DateTime LaunchDateUtc,
    DateTime? EndDateUtc,
    Guid OwnerId,
    bool IsAutomated);
