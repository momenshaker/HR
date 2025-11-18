using HR.Domain.Entities;

namespace HR.Application.DTOs;

public sealed record PerformanceCycleDto(
    Guid Id,
    string Name,
    string Description,
    DateOnly PeriodStart,
    DateOnly PeriodEnd,
    DateOnly SelfEvaluationStart,
    DateOnly SelfEvaluationEnd,
    DateOnly ManagerEvaluationStart,
    DateOnly ManagerEvaluationEnd,
    PerformanceCycleStatus Status,
    Guid TemplateId,
    Guid RatingScaleId,
    IReadOnlyCollection<PerformanceCycleAssignmentDto> IncludedEmployees,
    DateTime CreatedAt,
    Guid CreatedBy,
    int EvaluationCount
);

public sealed record PerformanceCycleAssignmentDto(Guid EmployeeId, Guid? ManagerId, string Department);
