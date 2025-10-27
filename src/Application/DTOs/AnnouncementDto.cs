namespace HR.Application.DTOs;

/// <summary>
///     Read model representing an internal communication announcement.
/// </summary>
public sealed record AnnouncementDto(
    Guid Id,
    string Title,
    string Message,
    string Audience,
    Guid CreatedBy,
    DateTime PublishedAtUtc,
    bool RequiresAcknowledgement);
