namespace HR.Application.Abstractions.Services;

/// <summary>
///     Records auditable events performed through the API.
/// </summary>
public interface IAuditLogger
{
    Task LogAsync(AuditLogEntry entry, CancellationToken cancellationToken);
}

/// <summary>
///     Representation of an auditable action executed by an actor.
/// </summary>
/// <param name="ActorId">Identifier of the authenticated actor.</param>
/// <param name="Action">The action performed (create/update/delete).</param>
/// <param name="Entity">The entity type affected.</param>
/// <param name="EntityId">Identifier of the entity instance.</param>
/// <param name="TraceId">Correlation trace identifier.</param>
public sealed record AuditLogEntry(string ActorId, string Action, string Entity, string EntityId, string TraceId);
