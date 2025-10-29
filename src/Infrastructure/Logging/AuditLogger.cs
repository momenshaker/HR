using HR.Application.Abstractions.Services;
using Microsoft.Extensions.Logging;

namespace HR.Infrastructure.Logging;

/// <summary>
///     Simple audit logger that writes structured events to the configured logging pipeline.
/// </summary>
public sealed class AuditLogger(ILogger<AuditLogger> logger) : IAuditLogger
{
    private readonly ILogger<AuditLogger> _logger = logger;

    public Task LogAsync(AuditLogEntry entry, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _logger.LogInformation(
            "Audit log - actor: {ActorId}, action: {Action}, entity: {Entity}, entityId: {EntityId}, traceId: {TraceId}",
            entry.ActorId,
            entry.Action,
            entry.Entity,
            entry.EntityId,
            entry.TraceId
        );

        return Task.CompletedTask;
    }
}
