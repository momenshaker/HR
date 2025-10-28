using System.Collections.Concurrent;
using HR.Application.Abstractions.Repositories;
using HR.Domain.Entities;

namespace HR.Infrastructure.Persistence.Repositories;

/// <summary>
///     In-memory repository implementation for engagement campaigns.
/// </summary>
public sealed class InMemoryEngagementCampaignRepository : IEngagementCampaignRepository
{
    private readonly ConcurrentDictionary<Guid, EngagementCampaign> _campaigns = new();

    public Task<IReadOnlyCollection<EngagementCampaign>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        IReadOnlyCollection<EngagementCampaign> snapshot = _campaigns.Values.ToList();
        return Task.FromResult(snapshot);
    }

    public Task<EngagementCampaign?> GetByIdAsync(Guid campaignId, CancellationToken cancellationToken = default)
    {
        _campaigns.TryGetValue(campaignId, out var campaign);
        return Task.FromResult(campaign);
    }

    public Task<EngagementCampaign> AddAsync(EngagementCampaign campaign, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(campaign);

        if (!_campaigns.TryAdd(campaign.Id, campaign))
        {
            throw new InvalidOperationException($"An engagement campaign with id '{campaign.Id}' already exists.");
        }

        return Task.FromResult(campaign);
    }

    public Task<EngagementCampaign?> UpdateAsync(EngagementCampaign campaign, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(campaign);

        if (!_campaigns.ContainsKey(campaign.Id))
        {
            return Task.FromResult<EngagementCampaign?>(null);
        }

        _campaigns[campaign.Id] = campaign;
        return Task.FromResult<EngagementCampaign?>(campaign);
    }

    public Task<bool> RemoveAsync(Guid campaignId, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_campaigns.TryRemove(campaignId, out _));
    }
}
