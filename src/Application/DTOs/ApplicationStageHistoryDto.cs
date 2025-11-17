using System;

namespace HR.Application.DTOs;

/// <summary>
///     Read model capturing the change history for an application.
/// </summary>
public sealed record ApplicationStageHistoryDto(
    Guid Id,
    Guid ApplicationId,
    string FromStage,
    string ToStage,
    Guid ChangedBy,
    DateTime ChangedAt,
    string Reason);
