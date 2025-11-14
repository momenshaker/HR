namespace HR.Application.DTOs;

/// <summary>
///     Read model describing a work schedule and its configured shifts.
/// </summary>
public sealed record WorkScheduleDto(
    Guid Id,
    string Name,
    Guid? OrganizationId,
    Guid? DepartmentId,
    bool IsDefaultForOrganization,
    string TimeZoneId,
    IReadOnlyCollection<ShiftTemplateDto> ShiftTemplates);
