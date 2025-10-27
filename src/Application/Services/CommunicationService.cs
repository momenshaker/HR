using HR.Application.Abstractions.Repositories;
using HR.Application.Abstractions.Services;
using HR.Application.DTOs;
using HR.Application.Mappings;

namespace HR.Application.Services;

/// <inheritdoc />
public sealed class CommunicationService : ICommunicationService
{
    private readonly IAnnouncementRepository _announcementRepository;

    public CommunicationService(IAnnouncementRepository announcementRepository)
    {
        _announcementRepository = announcementRepository;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<AnnouncementDto>> GetAsync(CancellationToken cancellationToken = default)
    {
        var announcements = await _announcementRepository.GetAllAsync(cancellationToken).ConfigureAwait(false);
        return announcements.Select(announcement => announcement.ToDto()).ToArray();
    }

    /// <inheritdoc />
    public async Task<AnnouncementDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var announcement = await _announcementRepository.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
        return announcement?.ToDto();
    }

    /// <inheritdoc />
    public async Task<AnnouncementDto> CreateAsync(CreateAnnouncementRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var entity = request.ToEntity();
        var created = await _announcementRepository.AddAsync(entity, cancellationToken).ConfigureAwait(false);

        return created.ToDto();
    }

    /// <inheritdoc />
    public async Task<AnnouncementDto?> UpdateAsync(Guid id, UpdateAnnouncementRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var existing = await _announcementRepository.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
        if (existing is null)
        {
            return null;
        }

        var updatedEntity = request.ApplyUpdates(existing);
        var persisted = await _announcementRepository.UpdateAsync(updatedEntity, cancellationToken).ConfigureAwait(false);

        return persisted?.ToDto();
    }

    /// <inheritdoc />
    public Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return _announcementRepository.RemoveAsync(id, cancellationToken);
    }
}
