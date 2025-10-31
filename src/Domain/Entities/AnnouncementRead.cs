namespace HR.Domain.Entities;

/// <summary>
///     Read-receipt for a specific employee and announcement.
/// </summary>
public sealed class AnnouncementRead
{
    public Guid AnnouncementId { get; init; }

    public Guid EmployeeId { get; init; }

    public DateTime ReadAtUtc { get; init; }
}

