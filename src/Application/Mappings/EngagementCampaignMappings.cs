using HR.Application.DTOs;
using HR.Domain.Entities;

namespace HR.Application.Mappings;

/// <summary>
///     Mapping helpers for <see cref="EngagementCampaign" /> entities.
/// </summary>
public static class EngagementCampaignMappings
{
    public static EngagementCampaignDto ToDto(this EngagementCampaign campaign)
    {
        ArgumentNullException.ThrowIfNull(campaign);

        return new EngagementCampaignDto(
            campaign.Id,
            campaign.Name,
            campaign.Description,
            campaign.Channels,
            campaign.TargetAudience,
            campaign.LaunchDateUtc,
            campaign.EndDateUtc,
            campaign.OwnerId,
            campaign.IsAutomated);
    }

    public static EngagementCampaign ToEntity(this CreateEngagementCampaignRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        return new EngagementCampaign
        {
            Id = Guid.NewGuid(),
            Name = request.Name.Trim(),
            Description = request.Description.Trim(),
            Channels = request.Channels.Trim(),
            TargetAudience = request.TargetAudience.Trim(),
            LaunchDateUtc = request.LaunchDateUtc,
            EndDateUtc = request.EndDateUtc,
            OwnerId = request.OwnerId,
            IsAutomated = request.IsAutomated
        };
    }

    public static EngagementCampaign ApplyUpdates(this UpdateEngagementCampaignRequest request, EngagementCampaign existing)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(existing);

        return new EngagementCampaign
        {
            Id = existing.Id,
            Name = request.Name.Trim(),
            Description = request.Description.Trim(),
            Channels = request.Channels.Trim(),
            TargetAudience = request.TargetAudience.Trim(),
            LaunchDateUtc = request.LaunchDateUtc,
            EndDateUtc = request.EndDateUtc,
            OwnerId = request.OwnerId,
            IsAutomated = request.IsAutomated
        };
    }
}
