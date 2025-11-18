using HR.Application.Validation;

namespace HR.Application.DTOs;

public sealed class SubmitEvaluationRequest : IValidatableRequest
{
    public string Comments { get; init; } = string.Empty;

    public IReadOnlyCollection<SubmittedSectionRequest> Sections { get; init; } = Array.Empty<SubmittedSectionRequest>();

    public IReadOnlyCollection<SubmittedGoalRequest> Goals { get; init; } = Array.Empty<SubmittedGoalRequest>();
}

public sealed class SubmittedSectionRequest
{
    public Guid SectionId { get; init; }

    public IReadOnlyCollection<SubmittedItemRequest> Items { get; init; } = Array.Empty<SubmittedItemRequest>();
}

public sealed class SubmittedItemRequest
{
    public Guid ItemId { get; init; }

    public decimal? Score { get; init; }

    public string Comment { get; init; } = string.Empty;
}

public sealed class SubmittedGoalRequest
{
    public Guid GoalId { get; init; }

    public decimal? Score { get; init; }

    public decimal? ActualValue { get; init; }

    public string Comment { get; init; } = string.Empty;
}
