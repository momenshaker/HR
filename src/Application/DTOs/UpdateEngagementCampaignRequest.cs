using HR.Application.Validation;
namespace HR.Application.DTOs;

/// <summary>
///     Request payload for updating an engagement campaign.
/// </summary>
public sealed class UpdateEngagementCampaignRequest : IValidatableRequest
{
    public string Name { get; init; } = string.Empty;

    public string Description { get; init; } = string.Empty;

    public string Channels { get; init; } = string.Empty;

    public string TargetAudience { get; init; } = string.Empty;

    public DateTime LaunchDateUtc { get; init; }

    public DateTime? EndDateUtc { get; init; }

    public Guid OwnerId { get; init; }

    public bool IsAutomated { get; init; }
}