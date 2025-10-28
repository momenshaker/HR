namespace HR.Domain.Entities;

/// <summary>
///     Represents an employee engagement campaign distributed across multiple channels.
/// </summary>
public sealed class EngagementCampaign
{
    public Guid Id { get; init; }

    public string Name { get; init; } = string.Empty;

    public string Description { get; init; } = string.Empty;

    public string Channels { get; init; } = string.Empty;

    public string TargetAudience { get; init; } = string.Empty;

    public DateTime LaunchDateUtc { get; init; }

    public DateTime? EndDateUtc { get; init; }

    public Guid OwnerId { get; init; }

    public bool IsAutomated { get; init; }
}
