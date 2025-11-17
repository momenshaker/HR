using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using HR.Application.Validation;

namespace HR.Application.DTOs;

/// <summary>
///     Incoming payload for creating a job requisition.
/// </summary>
public sealed class CreateJobRequisitionRequest : IValidatableRequest
{
    [Required]
    [MaxLength(200)]
    public string Title { get; init; } = string.Empty;

    [Required]
    public Guid DepartmentId { get; init; }

    [Required]
    public Guid HiringManagerId { get; init; }

    [Required]
    public Guid RequestedById { get; init; }

    [Range(1, 1000)]
    public int NumberOfPositions { get; init; } = 1;

    [Required]
    [MaxLength(100)]
    public string EmploymentType { get; init; } = string.Empty;

    [Required]
    [MaxLength(150)]
    public string Location { get; init; } = string.Empty;

    public decimal? BudgetedSalaryMin { get; init; }

    public decimal? BudgetedSalaryMax { get; init; }

    [MaxLength(4000)]
    public string Description { get; init; } = string.Empty;

    [MaxLength(50)]
    public string Status { get; init; } = string.Empty;

    public IReadOnlyCollection<string> ApprovalWorkflow { get; init; } = Array.Empty<string>();
}
