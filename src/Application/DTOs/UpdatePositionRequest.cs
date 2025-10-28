using System.ComponentModel.DataAnnotations;

namespace HR.Application.DTOs;

/// <summary>
///     Incoming request payload for updating an existing organisation position.
/// </summary>
public sealed class UpdatePositionRequest
{
    [Required]
    [MaxLength(150)]
    public string Title { get; init; } = string.Empty;

    [Required]
    [MaxLength(30)]
    public string JobCode { get; init; } = string.Empty;

    [Required]
    public Guid OrganizationUnitId { get; init; }

    public Guid? ReportsToPositionId { get; init; }

    public Guid? OccupiedByEmployeeId { get; init; }

    [MaxLength(20)]
    public string Grade { get; init; } = string.Empty;

    [MaxLength(40)]
    public string EmploymentType { get; init; } = string.Empty;

    public DateOnly? EffectiveFrom { get; init; }

    public DateOnly? EffectiveTo { get; init; }

    public bool IsCriticalRole { get; init; }

    public bool IsVacant { get; init; }
}
