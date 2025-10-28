using System.ComponentModel.DataAnnotations;

namespace HR.Application.DTOs;

/// <summary>
///     Request payload for advancing a candidate through the pipeline.
/// </summary>
public sealed class AdvanceCandidateRequest
{
    [Required]
    [MaxLength(100)]
    public string TargetStage { get; init; } = string.Empty;

    public DateTime? NextFollowUpAtUtc { get; init; }

    [MaxLength(2000)]
    public string Notes { get; init; } = string.Empty;

    public ScheduleInterviewRequest? Interview { get; init; }
}
