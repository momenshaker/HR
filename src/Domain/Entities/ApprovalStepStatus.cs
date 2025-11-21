namespace HR.Domain.Entities;

/// <summary>
///     Well-known statuses for leave approval workflow steps.
/// </summary>
public static class ApprovalStepStatus
{
    public const string Pending = nameof(Pending);
    public const string Approved = nameof(Approved);
    public const string Rejected = nameof(Rejected);
    public const string NotStarted = nameof(NotStarted);
}
