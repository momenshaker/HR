using System.ComponentModel.DataAnnotations;

namespace HR.Application.DTOs;

/// <summary>
///     Request payload representing a goal within a performance review.
/// </summary>
public sealed class PerformanceGoalRequest
{
    public Guid? Id { get; init; }

    [Required]
    [MaxLength(200)]
    public string Title { get; init; } = string.Empty;

    [MaxLength(2000)]
    public string Description { get; init; } = string.Empty;

    [Range(0, 100)]
    public decimal Weight { get; init; }

    public Guid? ParentGoalId { get; init; }

    [MaxLength(100)]
    public string Alignment { get; init; } = string.Empty;

    [MaxLength(50)]
    public string Status { get; init; } = string.Empty;
}
