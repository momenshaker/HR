using HR.Application.Abstractions.Repositories;
using HR.Domain.Entities;

namespace HR.Infrastructure.Persistence.EntityFramework.Repositories;

internal sealed class EntityFrameworkEngagementCampaignRepository
    : EntityFrameworkRepository<EngagementCampaign>, IEngagementCampaignRepository
{
    public EntityFrameworkEngagementCampaignRepository(HrDbContext dbContext)
        : base(dbContext, campaign => campaign.Id)
    {
    }

    public async Task<IReadOnlyCollection<EngagementCampaign>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await GetAllInternalAsync(cancellationToken).ConfigureAwait(false);
    }

    public Task<EngagementCampaign?> GetByIdAsync(Guid campaignId, CancellationToken cancellationToken = default)
    {
        return GetByIdInternalAsync(campaignId, cancellationToken);
    }

    public Task<EngagementCampaign> AddAsync(EngagementCampaign campaign, CancellationToken cancellationToken = default)
    {
        return AddInternalAsync(campaign, cancellationToken);
    }

    public Task<EngagementCampaign?> UpdateAsync(EngagementCampaign campaign, CancellationToken cancellationToken = default)
    {
        return UpdateInternalAsync(campaign, cancellationToken);
    }

    public Task<bool> RemoveAsync(Guid campaignId, CancellationToken cancellationToken = default)
    {
        return RemoveInternalAsync(campaignId, cancellationToken);
    }
}
