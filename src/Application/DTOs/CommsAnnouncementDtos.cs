using System.ComponentModel.DataAnnotations;
using HR.Application.Validation;

namespace HR.Application.DTOs;

public sealed record CommsAnnouncementDto(
    Guid Id,
    Guid OrganizationId,
    Guid? DepartmentId,
    string Title,
    string Body,
    DateTime PublishedAtUtc,
    Guid PublishedById,
    bool IsPinned);

public sealed class CreateCommsAnnouncementRequest : IValidatableRequest
{
    [Required]
    public Guid OrganizationId { get; init; }

    public Guid? DepartmentId { get; init; }

    [Required]
    [MaxLength(200)]
    public string Title { get; init; } = string.Empty;

    [Required]
    [MaxLength(5000)]
    public string Body { get; init; } = string.Empty;

    [Required]
    public Guid PublishedById { get; init; }
}
