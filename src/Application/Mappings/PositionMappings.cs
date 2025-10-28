using HR.Application.DTOs;
using HR.Domain.Entities;

namespace HR.Application.Mappings;

/// <summary>
///     Mapping helpers for <see cref="Position" /> entities.
/// </summary>
public static class PositionMappings
{
    public static PositionDto ToDto(this Position position)
    {
        ArgumentNullException.ThrowIfNull(position);

        return new PositionDto(
            position.Id,
            position.Title,
            position.JobCode,
            position.OrganizationUnitId,
            position.ReportsToPositionId,
            position.OccupiedByEmployeeId,
            position.Grade,
            position.EmploymentType,
            position.EffectiveFrom,
            position.EffectiveTo,
            position.IsCriticalRole,
            position.IsVacant);
    }

    public static Position ToEntity(this CreatePositionRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        return new Position
        {
            Id = Guid.NewGuid(),
            Title = request.Title.Trim(),
            JobCode = request.JobCode.Trim().ToUpperInvariant(),
            OrganizationUnitId = request.OrganizationUnitId,
            ReportsToPositionId = request.ReportsToPositionId,
            OccupiedByEmployeeId = request.OccupiedByEmployeeId,
            Grade = request.Grade.Trim().ToUpperInvariant(),
            EmploymentType = request.EmploymentType.Trim(),
            EffectiveFrom = request.EffectiveFrom,
            EffectiveTo = request.EffectiveTo,
            IsCriticalRole = request.IsCriticalRole,
            IsVacant = request.IsVacant
        };
    }

    public static Position ApplyUpdates(this UpdatePositionRequest request, Position existing)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(existing);

        return new Position
        {
            Id = existing.Id,
            Title = request.Title.Trim(),
            JobCode = request.JobCode.Trim().ToUpperInvariant(),
            OrganizationUnitId = request.OrganizationUnitId,
            ReportsToPositionId = request.ReportsToPositionId,
            OccupiedByEmployeeId = request.OccupiedByEmployeeId,
            Grade = request.Grade.Trim().ToUpperInvariant(),
            EmploymentType = request.EmploymentType.Trim(),
            EffectiveFrom = request.EffectiveFrom,
            EffectiveTo = request.EffectiveTo,
            IsCriticalRole = request.IsCriticalRole,
            IsVacant = request.IsVacant
        };
    }
}
