using System.ComponentModel.DataAnnotations;
using HR.Application.Validation;

namespace HR.Application.DTOs;

/// <summary>
///     Payload for updating an existing work schedule.
/// </summary>
public sealed class UpdateWorkScheduleRequest : IValidatableRequest
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
