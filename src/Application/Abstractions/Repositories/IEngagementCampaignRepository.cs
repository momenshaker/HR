using HR.Domain.Entities;

namespace HR.Application.Abstractions.Repositories;

/// <summary>
///     Repository contract for managing <see cref="EngagementCampaign" /> aggregates.
/// </summary>
public interface IEngagementCampaignRepository
{
    Task<IReadOnlyCollection<EngagementCampaign>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<EngagementCampaign?> GetByIdAsync(Guid campaignId, CancellationToken cancellationToken = default);

    Task<EngagementCampaign> AddAsync(EngagementCampaign campaign, CancellationToken cancellationToken = default);

    Task<EngagementCampaign?> UpdateAsync(EngagementCampaign campaign, CancellationToken cancellationToken = default);

    Task<bool> RemoveAsync(Guid campaignId, CancellationToken cancellationToken = default);
}
