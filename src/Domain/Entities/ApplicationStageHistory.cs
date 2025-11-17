using System;

namespace HR.Domain.Entities;

/// <summary>
///     Captures the movement of an application across pipeline stages.
/// </summary>
public sealed class ApplicationStageHistory
{
    public Guid Id { get; init; }

    public Guid ApplicationId { get; init; }

    public string FromStage { get; init; } = string.Empty;

    public string ToStage { get; init; } = string.Empty;

    public Guid ChangedBy { get; init; }

    public DateTime ChangedAt { get; init; }

    public string Reason { get; init; } = string.Empty;
}
