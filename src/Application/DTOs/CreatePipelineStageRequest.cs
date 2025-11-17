using System;
using System.ComponentModel.DataAnnotations;
using HR.Application.Validation;

namespace HR.Application.DTOs;

/// <summary>
///     Incoming payload for defining a pipeline stage.
/// </summary>
public sealed class CreatePipelineStageRequest : IValidatableRequest
{
    public Guid? JobPostingId { get; init; }

    public bool IsDefault { get; init; }

    [Required]
    [MaxLength(100)]
    public string Name { get; init; } = string.Empty;

    [Range(0, 100)]
    public int Order { get; init; }

    public bool IsFinalStage { get; init; }

    [MaxLength(500)]
    public string AutoEmailTemplateOnEnter { get; init; } = string.Empty;
}
