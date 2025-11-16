using System.ComponentModel.DataAnnotations;
using HR.Application.Validation;

namespace HR.Application.DTOs;

/// <summary>
///     Payload for creating a work schedule with one or more shifts.
/// </summary>
public sealed class CreateWorkScheduleRequest : IValidatableRequest
{
    [Required]
    [MaxLength(200)]
    public string Name { get; init; } = string.Empty;

    public Guid? OrganizationId { get; init; }

    public Guid? DepartmentId { get; init; }

    public bool IsDefaultForOrganization { get; init; }

    [MaxLength(100)]
    public string TimeZoneId { get; init; } = string.Empty;

    public IReadOnlyCollection<ShiftTemplateRequest> ShiftTemplates { get; init; } = Array.Empty<ShiftTemplateRequest>();
}
