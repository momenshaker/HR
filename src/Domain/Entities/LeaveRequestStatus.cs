namespace HR.Domain.Entities;

/// <summary>
///     Well-known leave workflow statuses used across services and integrations.
/// </summary>
public static class LeaveRequestStatus
{
    public const string Draft = nameof(Draft);
    public const string PendingApproval = nameof(PendingApproval);
    public const string Approved = nameof(Approved);
    public const string Rejected = nameof(Rejected);
    public const string Cancelled = nameof(Cancelled);
}
