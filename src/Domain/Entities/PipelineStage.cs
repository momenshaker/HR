using System;

namespace HR.Domain.Entities;

/// <summary>
///     Defines a recruitment pipeline stage for a job posting or global default.
/// </summary>
public sealed class PipelineStage
{
    public Guid Id { get; init; }

    public Guid? JobPostingId { get; init; }

    public bool IsDefault { get; init; }

    public string Name { get; init; } = string.Empty;

    public int Order { get; init; }

    public bool IsFinalStage { get; init; }

    public string AutoEmailTemplateOnEnter { get; init; } = string.Empty;
}
