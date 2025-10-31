using HR.Application.DTOs;
using HR.Domain.Entities;

namespace HR.Application.Mappings;

public static class CommsAnnouncementMappings
{
    public static CommsAnnouncementDto ToDto(this CommsAnnouncement entity)
    {
        return new CommsAnnouncementDto(
            entity.Id,
            entity.OrganizationId,
            entity.DepartmentId,
            entity.Title,
            entity.Body,
            entity.PublishedAtUtc,
            entity.PublishedById,
            entity.IsPinned);
    }

    public static CommsAnnouncement ToEntity(this CreateCommsAnnouncementRequest request, DateTime publishedAtUtc)
    {
        ArgumentNullException.ThrowIfNull(request);
        return new CommsAnnouncement
        {
            Id = Guid.NewGuid(),
            OrganizationId = request.OrganizationId,
            DepartmentId = request.DepartmentId,
            Title = request.Title.Trim(),
            Body = request.Body.Trim(),
            PublishedAtUtc = publishedAtUtc,
            PublishedById = request.PublishedById,
            IsPinned = false
        };
    }
}

