using System;
using System.Collections.Generic;

namespace HR.Domain.Entities;

/// <summary>
///     Represents the internal request to open a hiring process.
/// </summary>
public sealed class JobRequisition
{
    public Guid Id { get; init; }

    public string Title { get; init; } = string.Empty;

    public Guid DepartmentId { get; init; }

    public Guid HiringManagerId { get; init; }

    public Guid RequestedById { get; init; }

    public int NumberOfPositions { get; init; }

    public string EmploymentType { get; init; } = string.Empty;

    public string Location { get; init; } = string.Empty;

    public decimal? BudgetedSalaryMin { get; init; }

    public decimal? BudgetedSalaryMax { get; init; }

    public string Description { get; init; } = string.Empty;

    public string Status { get; init; } = string.Empty;

    public List<string> ApprovalWorkflow { get; init; } = new();
}
