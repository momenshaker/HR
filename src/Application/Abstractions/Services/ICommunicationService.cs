using HR.Application.DTOs;

namespace HR.Application.Abstractions.Services;

/// <summary>
///     Application service responsible for internal communication operations.
/// </summary>
public interface ICommunicationService
{
    Task<IReadOnlyCollection<AnnouncementDto>> GetAsync(CancellationToken cancellationToken = default);

    Task<AnnouncementDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<AnnouncementDto> CreateAsync(CreateAnnouncementRequest request, CancellationToken cancellationToken = default);

    Task<AnnouncementDto?> UpdateAsync(Guid id, UpdateAnnouncementRequest request, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
