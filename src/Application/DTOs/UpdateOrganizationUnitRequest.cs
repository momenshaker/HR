using System.ComponentModel.DataAnnotations;
using HR.Application.Validation;

namespace HR.Application.DTOs;

/// <summary>
///     Incoming request payload for updating an existing organisation unit.
/// </summary>
public sealed class UpdateOrganizationUnitRequest : IValidatableRequest
{
    [Required]
    [MaxLength(150)]
    public string Name { get; init; } = string.Empty;

    [Required]
    [MaxLength(30)]
    public string Code { get; init; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string Type { get; init; } = string.Empty;

    public Guid? ParentUnitId { get; init; }

    public Guid? DepartmentId { get; init; }

    public Guid? LeadPositionId { get; init; }

    [Range(0, 25)]
    public int Level { get; init; }

    [MaxLength(500)]
    public string Description { get; init; } = string.Empty;

    public bool IsActive { get; init; } = true;
}