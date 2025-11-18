using HR.Application.Validation;

namespace HR.Application.DTOs;

public sealed class CreatePerformanceCycleRequest : IValidatableRequest
{
    public string Name { get; init; } = string.Empty;

    public string Description { get; init; } = string.Empty;

    public DateOnly PeriodStart { get; init; }

    public DateOnly PeriodEnd { get; init; }

    public DateOnly SelfEvaluationStart { get; init; }

    public DateOnly SelfEvaluationEnd { get; init; }

    public DateOnly ManagerEvaluationStart { get; init; }

    public DateOnly ManagerEvaluationEnd { get; init; }

    public Guid TemplateId { get; init; }

    public Guid RatingScaleId { get; init; }

    public Guid CreatedBy { get; init; }

    public IReadOnlyCollection<PerformanceCycleAssignmentRequest> IncludedEmployees { get; init; } = Array.Empty<PerformanceCycleAssignmentRequest>();
}

public sealed class PerformanceCycleAssignmentRequest
{
    public Guid EmployeeId { get; init; }

    public Guid? ManagerId { get; init; }

    public string Department { get; init; } = string.Empty;
}
