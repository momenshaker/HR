using HR.Domain.Entities;

namespace HR.Application.Abstractions.Repositories;

/// <summary>
///     Repository contract for managing <see cref="Announcement" /> aggregates.
/// </summary>
public interface IAnnouncementRepository
{
    Task<IReadOnlyCollection<Announcement>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<Announcement?> GetByIdAsync(Guid announcementId, CancellationToken cancellationToken = default);

    Task<Announcement> AddAsync(Announcement announcement, CancellationToken cancellationToken = default);

    Task<Announcement?> UpdateAsync(Announcement announcement, CancellationToken cancellationToken = default);

    Task<bool> RemoveAsync(Guid announcementId, CancellationToken cancellationToken = default);
}
