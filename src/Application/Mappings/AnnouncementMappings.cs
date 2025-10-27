using HR.Application.DTOs;
using HR.Domain.Entities;

namespace HR.Application.Mappings;

/// <summary>
///     Mapping helpers for <see cref="Announcement" /> entities.
/// </summary>
public static class AnnouncementMappings
{
    public static AnnouncementDto ToDto(this Announcement announcement)
    {
        ArgumentNullException.ThrowIfNull(announcement);

        return new AnnouncementDto(
            announcement.Id,
            announcement.Title,
            announcement.Message,
            announcement.Audience,
            announcement.CreatedBy,
            announcement.PublishedAtUtc,
            announcement.RequiresAcknowledgement);
    }

    public static Announcement ToEntity(this CreateAnnouncementRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        return new Announcement
        {
            Id = Guid.NewGuid(),
            Title = request.Title.Trim(),
            Message = request.Message.Trim(),
            Audience = request.Audience.Trim(),
            CreatedBy = request.CreatedBy,
            PublishedAtUtc = DateTime.UtcNow,
            RequiresAcknowledgement = request.RequiresAcknowledgement
        };
    }

    public static Announcement ApplyUpdates(this UpdateAnnouncementRequest request, Announcement existing)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(existing);

        return new Announcement
        {
            Id = existing.Id,
            Title = request.Title.Trim(),
            Message = request.Message.Trim(),
            Audience = request.Audience.Trim(),
            CreatedBy = request.CreatedBy,
            PublishedAtUtc = request.PublishedAtUtc,
            RequiresAcknowledgement = request.RequiresAcknowledgement
        };
    }
}
