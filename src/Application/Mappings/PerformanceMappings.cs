using HR.Application.DTOs;
using HR.Domain.Entities;

namespace HR.Application.Mappings;

public static class PerformanceMappings
{
    public static RatingScaleDto ToDto(this RatingScale scale)
    {
        return new RatingScaleDto(
            scale.Id,
            scale.Name,
            scale.MinScore,
            scale.MaxScore,
            scale.AllowHalfPoints,
            scale.Levels.Select(level => new RatingScaleLevelDto(level.Id, level.RatingScaleId, level.Score, level.Label, level.Description)).ToArray()
        );
    }

    public static EvaluationTemplateDto ToDto(this EvaluationTemplate template)
    {
        return new EvaluationTemplateDto(
            template.Id,
            template.Name,
            template.Description,
            template.TargetRole,
            template.RatingScaleId,
            template.IsDefault,
            template.IsActive,
            template.Sections.Select(section => section.ToDto()).ToArray()
        );
    }

    public static TemplateSectionDefinitionDto ToDto(this TemplateSectionDefinition section)
    {
        return new TemplateSectionDefinitionDto(
            section.Id,
            section.TemplateId,
            section.Name,
            section.Weight,
            section.Items.Select(item => item.ToDto()).ToArray()
        );
    }

    public static TemplateItemDefinitionDto ToDto(this TemplateItemDefinition item)
    {
        return new TemplateItemDefinitionDto(item.Id, item.SectionDefinitionId, item.Name, item.Description, item.DefaultWeight);
    }

    public static EvaluationTemplate ToEntity(this CreateEvaluationTemplateRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        return new EvaluationTemplate
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Description = request.Description,
            TargetRole = request.TargetRole,
            RatingScaleId = request.RatingScaleId,
            IsDefault = request.IsDefault,
            IsActive = request.IsActive,
            Sections = request.Sections.Select((section, _) => section.ToEntity()).ToArray()
        };
    }

    public static TemplateSectionDefinition ToEntity(this TemplateSectionDefinitionRequest section)
    {
        ArgumentNullException.ThrowIfNull(section);

        var sectionId = Guid.NewGuid();

        return new TemplateSectionDefinition
        {
            Id = sectionId,
            TemplateId = Guid.Empty,
            Name = section.Name,
            Weight = section.Weight,
            Items = section.Items.Select(item => item.ToEntity(sectionId)).ToArray()
        };
    }

    public static TemplateItemDefinition ToEntity(this TemplateItemDefinitionRequest item, Guid sectionId)
    {
        ArgumentNullException.ThrowIfNull(item);

        return new TemplateItemDefinition
        {
            Id = Guid.NewGuid(),
            SectionDefinitionId = sectionId,
            Name = item.Name,
            Description = item.Description,
            DefaultWeight = item.DefaultWeight
        };
    }

    public static PerformanceCycle ToEntity(this CreatePerformanceCycleRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        return new PerformanceCycle
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Description = request.Description,
            PeriodStart = request.PeriodStart,
            PeriodEnd = request.PeriodEnd,
            SelfEvaluationStart = request.SelfEvaluationStart,
            SelfEvaluationEnd = request.SelfEvaluationEnd,
            ManagerEvaluationStart = request.ManagerEvaluationStart,
            ManagerEvaluationEnd = request.ManagerEvaluationEnd,
            TemplateId = request.TemplateId,
            RatingScaleId = request.RatingScaleId,
            CreatedBy = request.CreatedBy,
            CreatedAt = DateTime.UtcNow,
            IncludedEmployees = request.IncludedEmployees.Select(emp => emp.ToEntity()).ToArray()
        };
    }

    public static PerformanceCycle ToUpdatedEntity(this UpdatePerformanceCycleRequest request, PerformanceCycle existing)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(existing);

        return existing with
        {
            Name = request.Name,
            Description = request.Description,
            PeriodStart = request.PeriodStart,
            PeriodEnd = request.PeriodEnd,
            SelfEvaluationStart = request.SelfEvaluationStart,
            SelfEvaluationEnd = request.SelfEvaluationEnd,
            ManagerEvaluationStart = request.ManagerEvaluationStart,
            ManagerEvaluationEnd = request.ManagerEvaluationEnd,
            TemplateId = request.TemplateId,
            RatingScaleId = request.RatingScaleId,
            Status = request.Status,
            IncludedEmployees = request.IncludedEmployees.Select(emp => emp.ToEntity()).ToArray()
        };
    }

    public static PerformanceCycleAssignment ToEntity(this PerformanceCycleAssignmentRequest request)
    {
        return new PerformanceCycleAssignment
        {
            EmployeeId = request.EmployeeId,
            ManagerId = request.ManagerId,
            Department = request.Department
        };
    }

    public static PerformanceCycleDto ToDto(this PerformanceCycle cycle, int evaluationCount)
    {
        return new PerformanceCycleDto(
            cycle.Id,
            cycle.Name,
            cycle.Description,
            cycle.PeriodStart,
            cycle.PeriodEnd,
            cycle.SelfEvaluationStart,
            cycle.SelfEvaluationEnd,
            cycle.ManagerEvaluationStart,
            cycle.ManagerEvaluationEnd,
            cycle.Status,
            cycle.TemplateId,
            cycle.RatingScaleId,
            cycle.IncludedEmployees.Select(emp => new PerformanceCycleAssignmentDto(emp.EmployeeId, emp.ManagerId, emp.Department)).ToArray(),
            cycle.CreatedAt,
            cycle.CreatedBy,
            evaluationCount
        );
    }

    public static EvaluationDto ToDto(this Evaluation evaluation)
    {
        return new EvaluationDto(
            evaluation.Id,
            evaluation.EmployeeId,
            evaluation.ManagerId,
            evaluation.CycleId,
            evaluation.TemplateId,
            evaluation.OverallScore,
            evaluation.OverallRatingLevelId,
            evaluation.Status,
            evaluation.FinalCommentsEmployee,
            evaluation.FinalCommentsManager,
            evaluation.Sections.Select(section => section.ToDto()).ToArray(),
            evaluation.Goals.Select(goal => goal.ToDto()).ToArray(),
            evaluation.Participants.Select(participant => participant.ToDto()).ToArray(),
            evaluation.CreatedAt,
            evaluation.UpdatedAt
        );
    }

    public static EvaluationSummaryDto ToSummaryDto(this Evaluation evaluation, string cycleName, string templateName)
    {
        return new EvaluationSummaryDto(
            evaluation.Id,
            evaluation.EmployeeId,
            evaluation.ManagerId,
            evaluation.CycleId,
            evaluation.Status,
            evaluation.OverallScore,
            cycleName,
            templateName
        );
    }

    public static EvaluationSectionDto ToDto(this EvaluationSection section)
    {
        return new EvaluationSectionDto(
            section.Id,
            section.EvaluationId,
            section.TemplateSectionDefinitionId,
            section.Name,
            section.Weight,
            section.Score,
            section.Comments,
            section.Items.Select(item => item.ToDto()).ToArray()
        );
    }

    public static EvaluationItemDto ToDto(this EvaluationItem item)
    {
        return new EvaluationItemDto(
            item.Id,
            item.EvaluationSectionId,
            item.TemplateItemDefinitionId,
            item.Name,
            item.Weight,
            item.SelfScore,
            item.SelfComment,
            item.ManagerScore,
            item.ManagerComment,
            item.FinalScore
        );
    }

    public static EvaluationGoalDto ToDto(this EvaluationGoal goal)
    {
        return new EvaluationGoalDto(
            goal.Id,
            goal.EvaluationId,
            goal.GoalId,
            goal.Title,
            goal.Description,
            goal.Weight,
            goal.TargetValue,
            goal.ActualValue,
            goal.SelfScore,
            goal.ManagerScore,
            goal.FinalScore
        );
    }

    public static EvaluationParticipantDto ToDto(this EvaluationParticipant participant)
    {
        return new EvaluationParticipantDto(
            participant.Id,
            participant.EvaluationId,
            participant.ParticipantEmployeeId,
            participant.Role,
            participant.Status,
            participant.IsAnonymous
        );
    }

    public static Evaluation ApplySelfSubmission(this Evaluation evaluation, SubmitEvaluationRequest request)
    {
        var updatedSections = evaluation.Sections.Select(section => section.ApplySubmission(request.Sections, isManager: false)).ToArray();
        var updatedGoals = evaluation.Goals.Select(goal => goal.ApplySubmission(request.Goals, isManager: false)).ToArray();

        var updatedEvaluation = evaluation with
        {
            Sections = updatedSections,
            Goals = updatedGoals,
            FinalCommentsEmployee = request.Comments,
            Participants = UpdateParticipants(evaluation.Participants, EvaluationParticipantRole.Self, EvaluationStatus.SelfCompleted),
            Status = evaluation.Status < EvaluationStatus.SelfCompleted ? EvaluationStatus.SelfCompleted : evaluation.Status,
            UpdatedAt = DateTime.UtcNow
        };

        return updatedEvaluation.WithCalculatedScores();
    }

    public static Evaluation ApplyManagerSubmission(this Evaluation evaluation, SubmitEvaluationRequest request)
    {
        var updatedSections = evaluation.Sections.Select(section => section.ApplySubmission(request.Sections, isManager: true)).ToArray();
        var updatedGoals = evaluation.Goals.Select(goal => goal.ApplySubmission(request.Goals, isManager: true)).ToArray();

        var updatedEvaluation = evaluation with
        {
            Sections = updatedSections,
            Goals = updatedGoals,
            FinalCommentsManager = request.Comments,
            Participants = UpdateParticipants(evaluation.Participants, EvaluationParticipantRole.Manager, EvaluationStatus.ManagerCompleted),
            Status = EvaluationStatus.ManagerCompleted,
            UpdatedAt = DateTime.UtcNow
        };

        return updatedEvaluation.WithCalculatedScores();
    }

    internal static Evaluation WithCalculatedScores(this Evaluation evaluation)
    {
        var scoredSections = evaluation.Sections.Select(section => section.WithCalculatedScore()).ToArray();
        var scoredGoals = evaluation.Goals.Select(goal => goal).ToArray();

        var sectionsWeighted = scoredSections.Sum(section => section.Score * Math.Max(section.Weight, 1));
        var sectionsWeightTotal = scoredSections.Sum(section => Math.Max(section.Weight, 1));

        var goalsWeighted = scoredGoals.Sum(goal => (goal.FinalScore ?? goal.ManagerScore ?? goal.SelfScore ?? 0m) * Math.Max(goal.Weight, 1));
        var goalsWeightTotal = scoredGoals.Sum(goal => Math.Max(goal.Weight, 1));

        var sectionsAverage = sectionsWeightTotal > 0 ? sectionsWeighted / sectionsWeightTotal : 0;
        var goalsAverage = goalsWeightTotal > 0 ? goalsWeighted / goalsWeightTotal : 0;

        var overall = sectionsWeightTotal > 0 && goalsWeightTotal > 0
            ? (sectionsAverage * 0.6m) + (goalsAverage * 0.4m)
            : sectionsWeightTotal > 0
                ? sectionsAverage
                : goalsAverage;

        return evaluation with
        {
            Sections = scoredSections,
            Goals = scoredGoals,
            OverallScore = Math.Round(overall, 2, MidpointRounding.AwayFromZero)
        };
    }

    private static EvaluationSection WithCalculatedScore(this EvaluationSection section)
    {
        var weighted = section.Items.Sum(item => (item.FinalScore ?? item.ManagerScore ?? item.SelfScore ?? 0m) * Math.Max(item.Weight, 1));
        var weightTotal = section.Items.Sum(item => Math.Max(item.Weight, 1));
        var score = weightTotal > 0 ? weighted / weightTotal : 0;

        return section with { Score = Math.Round(score, 2, MidpointRounding.AwayFromZero) };
    }

    private static EvaluationSection ApplySubmission(this EvaluationSection section, IEnumerable<SubmittedSectionRequest> submissions, bool isManager)
    {
        var submission = submissions.FirstOrDefault(s => s.SectionId == section.Id);
        if (submission is null)
        {
            return section;
        }

        var updatedItems = section.Items.Select(item => item.ApplySubmission(submission.Items, isManager)).ToArray();
        return section with { Items = updatedItems };
    }

    private static EvaluationItem ApplySubmission(this EvaluationItem item, IEnumerable<SubmittedItemRequest> submissions, bool isManager)
    {
        var submission = submissions.FirstOrDefault(s => s.ItemId == item.Id);
        if (submission is null)
        {
            return item;
        }

        return isManager
            ? item with { ManagerScore = submission.Score, ManagerComment = submission.Comment }
            : item with { SelfScore = submission.Score, SelfComment = submission.Comment };
    }

    private static EvaluationGoal ApplySubmission(this EvaluationGoal goal, IEnumerable<SubmittedGoalRequest> submissions, bool isManager)
    {
        var submission = submissions.FirstOrDefault(s => s.GoalId == goal.Id);
        if (submission is null)
        {
            return goal;
        }

        if (isManager)
        {
            return goal with { ManagerScore = submission.Score, ActualValue = submission.ActualValue ?? goal.ActualValue };
        }

        return goal with { SelfScore = submission.Score, ActualValue = submission.ActualValue ?? goal.ActualValue };
    }

    private static IReadOnlyCollection<EvaluationParticipant> UpdateParticipants(
        IEnumerable<EvaluationParticipant> participants,
        EvaluationParticipantRole role,
        EvaluationStatus status)
    {
        return participants
            .Select(participant => participant.Role == role ? participant with { Status = status } : participant)
            .ToArray();
    }
}
