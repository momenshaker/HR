using System.ComponentModel.DataAnnotations;

namespace HR.Application.DTOs;

/// <summary>
///     Incoming request payload for updating a training course.
/// </summary>
public sealed class UpdateTrainingCourseRequest
{
    [Required]
    [MaxLength(200)]
    public string Title { get; init; } = string.Empty;

    [MaxLength(100)]
    public string Category { get; init; } = string.Empty;

    [MaxLength(2000)]
    public string Description { get; init; } = string.Empty;

    [MaxLength(150)]
    public string Instructor { get; init; } = string.Empty;

    [Required]
    public DateOnly StartDate { get; init; }

    [Required]
    public DateOnly EndDate { get; init; }

    [Range(0, 10000)]
    public int Capacity { get; init; }

    [MaxLength(50)]
    public string DeliveryMode { get; init; } = string.Empty;
}
