namespace HR.Domain.Entities;

/// <summary>
///     Represents an internal communication broadcasted to employees.
/// </summary>
public sealed class Announcement
{
    public Guid Id { get; init; }

    public string Title { get; init; } = string.Empty;

    public string Message { get; init; } = string.Empty;

    public string Audience { get; init; } = string.Empty;

    public Guid CreatedBy { get; init; }

    public DateTime PublishedAtUtc { get; init; }

    public bool RequiresAcknowledgement { get; init; }
}
