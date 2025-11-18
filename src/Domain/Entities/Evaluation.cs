namespace HR.Domain.Entities;

public enum EvaluationStatus
{
    NotStarted,
    SelfInProgress,
    SelfCompleted,
    ManagerInProgress,
    ManagerCompleted,
    Calibrated,
    Finalized,
    Cancelled
}

public enum EvaluationParticipantRole
{
    Self,
    Manager,
    Peer,
    Hr,
    SkipLevel
}

public sealed record Evaluation
{
    public Guid Id { get; init; }

    public Guid EmployeeId { get; init; }

    public Guid? ManagerId { get; init; }

    public Guid CycleId { get; init; }

    public Guid TemplateId { get; init; }

    public decimal OverallScore { get; init; }

    public Guid? OverallRatingLevelId { get; init; }

    public EvaluationStatus Status { get; init; } = EvaluationStatus.NotStarted;

    public string FinalCommentsEmployee { get; init; } = string.Empty;

    public string FinalCommentsManager { get; init; } = string.Empty;

    public IReadOnlyCollection<EvaluationSection> Sections { get; init; } = Array.Empty<EvaluationSection>();

    public IReadOnlyCollection<EvaluationGoal> Goals { get; init; } = Array.Empty<EvaluationGoal>();

    public IReadOnlyCollection<EvaluationParticipant> Participants { get; init; } = Array.Empty<EvaluationParticipant>();

    public DateTime CreatedAt { get; init; }

    public DateTime UpdatedAt { get; init; }
}

public sealed record EvaluationSection
{
    public Guid Id { get; init; }

    public Guid EvaluationId { get; init; }

    public Guid TemplateSectionDefinitionId { get; init; }

    public string Name { get; init; } = string.Empty;

    public decimal Weight { get; init; }

    public decimal Score { get; init; }

    public string Comments { get; init; } = string.Empty;

    public IReadOnlyCollection<EvaluationItem> Items { get; init; } = Array.Empty<EvaluationItem>();
}

public sealed record EvaluationItem
{
    public Guid Id { get; init; }

    public Guid EvaluationSectionId { get; init; }

    public Guid TemplateItemDefinitionId { get; init; }

    public string Name { get; init; } = string.Empty;

    public decimal Weight { get; init; }

    public decimal? SelfScore { get; init; }

    public string SelfComment { get; init; } = string.Empty;

    public decimal? ManagerScore { get; init; }

    public string ManagerComment { get; init; } = string.Empty;

    public decimal? FinalScore { get; init; }
}

public sealed record EvaluationGoal
{
    public Guid Id { get; init; }

    public Guid EvaluationId { get; init; }

    public Guid? GoalId { get; init; }

    public string Title { get; init; } = string.Empty;

    public string Description { get; init; } = string.Empty;

    public decimal Weight { get; init; }

    public decimal TargetValue { get; init; }

    public decimal ActualValue { get; init; }

    public decimal? SelfScore { get; init; }

    public decimal? ManagerScore { get; init; }

    public decimal? FinalScore { get; init; }
}

public sealed record EvaluationParticipant
{
    public Guid Id { get; init; }

    public Guid EvaluationId { get; init; }

    public Guid ParticipantEmployeeId { get; init; }

    public EvaluationParticipantRole Role { get; init; }

    public EvaluationStatus Status { get; init; }

    public bool IsAnonymous { get; init; }
}
