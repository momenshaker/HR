using System;
using System.ComponentModel.DataAnnotations;
using HR.Application.Validation;

namespace HR.Application.DTOs;

/// <summary>
///     Payload for recording a movement between pipeline stages.
/// </summary>
public sealed class RecordApplicationStageChangeRequest : IValidatableRequest
{
    [Required]
    public Guid ApplicationId { get; init; }

    [MaxLength(100)]
    public string FromStage { get; init; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string ToStage { get; init; } = string.Empty;

    [Required]
    public Guid ChangedBy { get; init; }

    [Required]
    public DateTime ChangedAt { get; init; }

    [MaxLength(1000)]
    public string Reason { get; init; } = string.Empty;
}
