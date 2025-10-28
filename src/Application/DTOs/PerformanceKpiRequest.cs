using System.ComponentModel.DataAnnotations;

namespace HR.Application.DTOs;

/// <summary>
///     Request payload representing a key performance indicator within a review.
/// </summary>
public sealed class PerformanceKpiRequest
{
    public Guid? Id { get; init; }

    [Required]
    [MaxLength(150)]
    public string Name { get; init; } = string.Empty;

    [Required]
    public decimal TargetValue { get; init; }

    public decimal ActualValue { get; init; }

    [MaxLength(50)]
    public string UnitOfMeasure { get; init; } = string.Empty;

    [MaxLength(50)]
    public string Status { get; init; } = string.Empty;
}
