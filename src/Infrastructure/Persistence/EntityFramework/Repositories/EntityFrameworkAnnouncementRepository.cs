using HR.Application.Abstractions.Repositories;
using HR.Domain.Entities;

namespace HR.Infrastructure.Persistence.EntityFramework.Repositories;

internal sealed class EntityFrameworkAnnouncementRepository : EntityFrameworkRepository<Announcement>, IAnnouncementRepository
{
    public EntityFrameworkAnnouncementRepository(HrDbContext dbContext)
        : base(dbContext, announcement => announcement.Id)
    {
    }

    public async Task<IReadOnlyCollection<Announcement>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await GetAllInternalAsync(cancellationToken).ConfigureAwait(false);
    }

    public Task<Announcement?> GetByIdAsync(Guid announcementId, CancellationToken cancellationToken = default)
    {
        return GetByIdInternalAsync(announcementId, cancellationToken);
    }

    public Task<Announcement> AddAsync(Announcement announcement, CancellationToken cancellationToken = default)
    {
        return AddInternalAsync(announcement, cancellationToken);
    }

    public Task<Announcement?> UpdateAsync(Announcement announcement, CancellationToken cancellationToken = default)
    {
        return UpdateInternalAsync(announcement, cancellationToken);
    }

    public Task<bool> RemoveAsync(Guid announcementId, CancellationToken cancellationToken = default)
    {
        return RemoveInternalAsync(announcementId, cancellationToken);
    }
}
