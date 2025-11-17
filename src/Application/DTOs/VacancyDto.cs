using System;
using System.Collections.Generic;

namespace HR.Application.DTOs;

/// <summary>
///     Read model representing a published vacancy.
/// </summary>
public sealed record VacancyDto(
    Guid Id,
    Guid RequisitionId,
    string PublicTitle,
    string PublicDescription,
    string Location,
    string WorkMode,
    string EmploymentType,
    bool SalaryVisible,
    string SalaryRangeText,
    int NumberOfPositions,
    string Department,
    IReadOnlyCollection<string> Responsibilities,
    IReadOnlyCollection<string> Requirements,
    IReadOnlyCollection<string> PostingChannels,
    IReadOnlyCollection<string> PipelineStages,
    IReadOnlyCollection<string> HiringTeam,
    DateTime CreatedAtUtc,
    DateTime? PublishedAtUtc,
    DateTime? ClosedAtUtc,
    string Status,
    string ApplicationUrl);
