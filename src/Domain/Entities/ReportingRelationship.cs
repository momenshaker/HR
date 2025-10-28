using System;

namespace HR.Domain.Entities;

/// <summary>
///     Represents a directional reporting relationship between positions in the organisation.
/// </summary>
public sealed class ReportingRelationship
{
    public Guid Id { get; init; }

    public Guid ManagerPositionId { get; init; }

    public Guid ReportPositionId { get; init; }

    public string RelationshipType { get; init; } = string.Empty;

    public DateOnly? EffectiveFrom { get; init; }

    public DateOnly? EffectiveTo { get; init; }

    public bool IsPrimary { get; init; }
}
