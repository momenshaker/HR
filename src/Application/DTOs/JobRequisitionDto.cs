using System;
using System.Collections.Generic;

namespace HR.Application.DTOs;

/// <summary>
///     Read model describing a job requisition.
/// </summary>
public sealed record JobRequisitionDto(
    Guid Id,
    string Title,
    Guid DepartmentId,
    Guid HiringManagerId,
    Guid RequestedById,
    int NumberOfPositions,
    string EmploymentType,
    string Location,
    decimal? BudgetedSalaryMin,
    decimal? BudgetedSalaryMax,
    string Description,
    string Status,
    IReadOnlyCollection<string> ApprovalWorkflow);
