using System.ComponentModel.DataAnnotations;

namespace HR.Application.DTOs;

public sealed record AttendancePunchConfigurationRequest
{
    public Guid? Id { get; init; }

    [Required]
    [MaxLength(100)]
    public string PunchType { get; init; } = string.Empty;

    [Required]
    [MaxLength(150)]
    public string DisplayName { get; init; } = string.Empty;

    [MaxLength(500)]
    public string Description { get; init; } = string.Empty;

    public int SortOrder { get; init; }

    public bool IsActive { get; init; } = true;
}
