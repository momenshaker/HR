using HR.Domain.Entities;

namespace HR.Application.DTOs;

public sealed record EvaluationSummaryDto(
    Guid Id,
    Guid EmployeeId,
    Guid? ManagerId,
    Guid CycleId,
    EvaluationStatus Status,
    decimal OverallScore,
    string CycleName,
    string TemplateName
);

public sealed record EvaluationDto(
    Guid Id,
    Guid EmployeeId,
    Guid? ManagerId,
    Guid CycleId,
    Guid TemplateId,
    decimal OverallScore,
    Guid? OverallRatingLevelId,
    EvaluationStatus Status,
    string FinalCommentsEmployee,
    string FinalCommentsManager,
    IReadOnlyCollection<EvaluationSectionDto> Sections,
    IReadOnlyCollection<EvaluationGoalDto> Goals,
    IReadOnlyCollection<EvaluationParticipantDto> Participants,
    DateTime CreatedAt,
    DateTime UpdatedAt
);

public sealed record EvaluationSectionDto(
    Guid Id,
    Guid EvaluationId,
    Guid TemplateSectionDefinitionId,
    string Name,
    decimal Weight,
    decimal Score,
    string Comments,
    IReadOnlyCollection<EvaluationItemDto> Items
);

public sealed record EvaluationItemDto(
    Guid Id,
    Guid EvaluationSectionId,
    Guid TemplateItemDefinitionId,
    string Name,
    decimal Weight,
    decimal? SelfScore,
    string SelfComment,
    decimal? ManagerScore,
    string ManagerComment,
    decimal? FinalScore
);

public sealed record EvaluationGoalDto(
    Guid Id,
    Guid EvaluationId,
    Guid? GoalId,
    string Title,
    string Description,
    decimal Weight,
    decimal TargetValue,
    decimal ActualValue,
    decimal? SelfScore,
    decimal? ManagerScore,
    decimal? FinalScore
);

public sealed record EvaluationParticipantDto(
    Guid Id,
    Guid EvaluationId,
    Guid ParticipantEmployeeId,
    EvaluationParticipantRole Role,
    EvaluationStatus Status,
    bool IsAnonymous
);
