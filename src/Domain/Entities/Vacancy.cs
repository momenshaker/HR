using System;
using System.Collections.Generic;

namespace HR.Domain.Entities;

/// <summary>
///     Represents a job vacancy that can be published externally for applicants.
/// </summary>
public sealed class Vacancy
{
    public Guid Id { get; init; }

    public Guid RequisitionId { get; init; }

    public string PublicTitle { get; init; } = string.Empty;

    public string PublicDescription { get; init; } = string.Empty;

    public string Location { get; init; } = string.Empty;

    public string WorkMode { get; init; } = string.Empty;

    public string EmploymentType { get; init; } = string.Empty;

    public bool SalaryVisible { get; init; }

    public string SalaryRangeText { get; init; } = string.Empty;

    public int NumberOfPositions { get; init; }

    public string Department { get; init; } = string.Empty;

    public List<string> Responsibilities { get; init; } = new();

    public List<string> Requirements { get; init; } = new();

    public List<string> PostingChannels { get; init; } = new();

    public List<string> PipelineStages { get; init; } = new();

    public List<string> HiringTeam { get; init; } = new();

    public DateTime CreatedAtUtc { get; init; }

    public DateTime? PublishedAtUtc { get; init; }

    public DateTime? ClosedAtUtc { get; init; }

    public string Status { get; init; } = string.Empty;

    public string ApplicationUrl { get; init; } = string.Empty;
}
