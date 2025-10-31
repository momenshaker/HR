using HR.Domain.Entities;

namespace HR.Application.Abstractions.Repositories;

public interface ICommsAnnouncementRepository
{
    Task<CommsAnnouncement> AddAsync(CommsAnnouncement entity, CancellationToken cancellationToken = default);
    Task<CommsAnnouncement?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<CommsAnnouncement?> UpdateAsync(CommsAnnouncement entity, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<CommsAnnouncement>> GetByOrganizationAsync(Guid organizationId, CancellationToken cancellationToken = default);
}

