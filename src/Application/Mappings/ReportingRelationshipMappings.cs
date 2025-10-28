using HR.Application.DTOs;
using HR.Domain.Entities;

namespace HR.Application.Mappings;

/// <summary>
///     Mapping helpers for <see cref="ReportingRelationship" /> entities.
/// </summary>
public static class ReportingRelationshipMappings
{
    public static ReportingRelationshipDto ToDto(this ReportingRelationship relationship)
    {
        ArgumentNullException.ThrowIfNull(relationship);

        return new ReportingRelationshipDto(
            relationship.Id,
            relationship.ManagerPositionId,
            relationship.ReportPositionId,
            relationship.RelationshipType,
            relationship.EffectiveFrom,
            relationship.EffectiveTo,
            relationship.IsPrimary);
    }

    public static ReportingRelationship ToEntity(this CreateReportingRelationshipRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        return new ReportingRelationship
        {
            Id = Guid.NewGuid(),
            ManagerPositionId = request.ManagerPositionId,
            ReportPositionId = request.ReportPositionId,
            RelationshipType = request.RelationshipType.Trim(),
            EffectiveFrom = request.EffectiveFrom,
            EffectiveTo = request.EffectiveTo,
            IsPrimary = request.IsPrimary
        };
    }

    public static ReportingRelationship ApplyUpdates(
        this UpdateReportingRelationshipRequest request,
        ReportingRelationship existing)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(existing);

        return new ReportingRelationship
        {
            Id = existing.Id,
            ManagerPositionId = request.ManagerPositionId,
            ReportPositionId = request.ReportPositionId,
            RelationshipType = request.RelationshipType.Trim(),
            EffectiveFrom = request.EffectiveFrom,
            EffectiveTo = request.EffectiveTo,
            IsPrimary = request.IsPrimary
        };
    }
}
