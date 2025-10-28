namespace HR.Application.DTOs;

/// <summary>
///     Read model describing a reporting relationship between two positions.
/// </summary>
public sealed record ReportingRelationshipDto(
    Guid Id,
    Guid ManagerPositionId,
    Guid ReportPositionId,
    string RelationshipType,
    DateOnly? EffectiveFrom,
    DateOnly? EffectiveTo,
    bool IsPrimary);
