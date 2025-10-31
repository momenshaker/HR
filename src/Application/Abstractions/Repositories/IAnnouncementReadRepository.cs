using HR.Domain.Entities;

namespace HR.Application.Abstractions.Repositories;

public interface IAnnouncementReadRepository
{
    Task MarkReadAsync(AnnouncementRead read, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<Guid>> GetReadAnnouncementIdsAsync(Guid employeeId, CancellationToken cancellationToken = default);
}

