using System.ComponentModel.DataAnnotations;

namespace HR.Application.DTOs;

/// <summary>
///     Incoming request payload for creating a department.
/// </summary>
public sealed class CreateDepartmentRequest
{
    [Required]
    [MaxLength(150)]
    public string Name { get; init; } = string.Empty;

    [Required]
    [MaxLength(20)]
    public string Code { get; init; } = string.Empty;

    public Guid? ParentDepartmentId { get; init; }

    public Guid? ManagerId { get; init; }

    [MaxLength(100)]
    public string Branch { get; init; } = string.Empty;

    [MaxLength(200)]
    public string Location { get; init; } = string.Empty;

    [MaxLength(500)]
    public string Description { get; init; } = string.Empty;

    public bool IsActive { get; init; } = true;
}
