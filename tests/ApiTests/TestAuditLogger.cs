using System.Collections.Concurrent;
using HR.Application.Abstractions.Services;

namespace HR.Api.IntegrationTests;

internal sealed class TestAuditLogger : IAuditLogger
{
    public ConcurrentBag<AuditLogEntry> Entries { get; } = new();

    public Task LogAsync(AuditLogEntry entry, CancellationToken cancellationToken)
    {
        Entries.Add(entry);
        return Task.CompletedTask;
    }
}
