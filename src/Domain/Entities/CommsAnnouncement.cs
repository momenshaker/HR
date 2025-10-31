namespace HR.Domain.Entities;

/// <summary>
///     Organization or department scoped announcement for employees.
/// </summary>
public sealed class CommsAnnouncement
{
    public Guid Id { get; init; }

    public Guid OrganizationId { get; init; }

    public Guid? DepartmentId { get; init; }

    public string Title { get; init; } = string.Empty;

    public string Body { get; init; } = string.Empty;

    public DateTime PublishedAtUtc { get; init; }

    public Guid PublishedById { get; init; }

    public bool IsPinned { get; init; }
}

