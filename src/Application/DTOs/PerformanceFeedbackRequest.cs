using System.ComponentModel.DataAnnotations;

namespace HR.Application.DTOs;

/// <summary>
///     Request payload representing structured feedback for a review cycle.
/// </summary>
public sealed class PerformanceFeedbackRequest
{
    public Guid? Id { get; init; }

    [Required]
    [MaxLength(50)]
    public string FeedbackType { get; init; } = string.Empty;

    [Required]
    [MaxLength(2000)]
    public string Comments { get; init; } = string.Empty;

    [Required]
    public Guid SubmittedBy { get; init; }

    public DateTime SubmittedAtUtc { get; init; } = DateTime.UtcNow;
}
