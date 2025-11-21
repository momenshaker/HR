namespace HR.Application.DTOs;

/// <summary>
///     Read model describing an organisation position.
/// </summary>
public sealed record PositionDto(
    Guid Id,
    string Title,
    string JobCode,
    Guid OrganizationUnitId,
    Guid? ReportsToPositionId,
    string Grade,
    string EmploymentType,
    DateOnly? EffectiveFrom,
    DateOnly? EffectiveTo,
    bool IsCriticalRole,
    bool IsVacant);
