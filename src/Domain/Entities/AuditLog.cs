namespace HR.Domain.Entities;

/// <summary>
///     Represents an audit trail entry capturing critical actions in the system.
/// </summary>
public sealed class AuditLog
{
    public Guid Id { get; init; }

    public Guid? CustomerId { get; init; }

    public string Actor { get; init; } = string.Empty;

    public string ActorEmail { get; init; } = string.Empty;

    public string EntityName { get; init; } = string.Empty;

    public string EntityId { get; init; } = string.Empty;

    public string Action { get; init; } = string.Empty;

    public string Severity { get; init; } = string.Empty;

    public string CorrelationId { get; init; } = string.Empty;

    public string Source { get; init; } = string.Empty;

    public string Changes { get; init; } = string.Empty;

    public string Metadata { get; init; } = string.Empty;

    public DateTimeOffset OccurredAtUtc { get; init; }
}
