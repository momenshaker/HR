using System;

namespace HR.Application.DTOs;

/// <summary>
///     Read model describing a recruitment pipeline stage.
/// </summary>
public sealed record PipelineStageDto(
    Guid Id,
    Guid? JobPostingId,
    bool IsDefault,
    string Name,
    int Order,
    bool IsFinalStage,
    string AutoEmailTemplateOnEnter);
