namespace HR.Domain.Entities;

public enum PerformanceCycleStatus
{
    Draft,
    Active,
    Closed,
    Archived
}

public sealed record PerformanceCycle
{
    public Guid Id { get; init; }

    public string Name { get; init; } = string.Empty;

    public string Description { get; init; } = string.Empty;

    public DateOnly PeriodStart { get; init; }

    public DateOnly PeriodEnd { get; init; }

    public DateOnly SelfEvaluationStart { get; init; }

    public DateOnly SelfEvaluationEnd { get; init; }

    public DateOnly ManagerEvaluationStart { get; init; }

    public DateOnly ManagerEvaluationEnd { get; init; }

    public PerformanceCycleStatus Status { get; init; } = PerformanceCycleStatus.Draft;

    public Guid TemplateId { get; init; }

    public Guid RatingScaleId { get; init; }

    public IReadOnlyCollection<PerformanceCycleAssignment> IncludedEmployees { get; init; } = Array.Empty<PerformanceCycleAssignment>();

    public Guid CreatedBy { get; init; }

    public DateTime CreatedAt { get; init; }
}

public sealed record PerformanceCycleAssignment
{
    public Guid EmployeeId { get; init; }

    public Guid? ManagerId { get; init; }

    public string Department { get; init; } = string.Empty;
}
