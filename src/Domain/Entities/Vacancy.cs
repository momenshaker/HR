using System;
using System.Collections.Generic;

namespace HR.Domain.Entities;

/// <summary>
///     Represents a job vacancy that can be published externally for applicants.
/// </summary>
public sealed class Vacancy
{
    public Guid Id { get; init; }

    public string Title { get; init; } = string.Empty;

    public string Department { get; init; } = string.Empty;

    public string Location { get; init; } = string.Empty;

    public string EmploymentType { get; init; } = string.Empty;

    public string Description { get; init; } = string.Empty;

    public List<string> Responsibilities { get; init; } = new();

    public List<string> Requirements { get; init; } = new();

    public List<string> PipelineStages { get; init; } = new();

    public List<string> HiringTeam { get; init; } = new();

    public DateTime PostedAtUtc { get; init; }

    public DateTime? ClosingAtUtc { get; init; }

    public string Status { get; init; } = string.Empty;

    public string ApplicationUrl { get; init; } = string.Empty;
}
