using System.ComponentModel.DataAnnotations;

namespace HR.Application.DTOs;

/// <summary>
///     Incoming request payload for updating an announcement.
/// </summary>
public sealed class UpdateAnnouncementRequest
{
    [Required]
    [MaxLength(200)]
    public string Title { get; init; } = string.Empty;

    [Required]
    [MaxLength(5000)]
    public string Message { get; init; } = string.Empty;

    [MaxLength(100)]
    public string Audience { get; init; } = string.Empty;

    [Required]
    public Guid CreatedBy { get; init; }

    public bool RequiresAcknowledgement { get; init; }

    public DateTime PublishedAtUtc { get; init; }
}
