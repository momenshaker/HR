using HR.Application.DTOs;
using HR.Domain.Entities;

namespace HR.Application.Mappings;

/// <summary>
///     Mapping helpers for <see cref="PerformanceReview" /> entities.
/// </summary>
public static class PerformanceReviewMappings
{
    public static PerformanceReviewDto ToDto(this PerformanceReview review)
    {
        ArgumentNullException.ThrowIfNull(review);

        return new PerformanceReviewDto(
            review.Id,
            review.EmployeeId,
            review.CycleName,
            review.PeriodStart,
            review.PeriodEnd,
            review.OverallScore,
            review.ManagerComments,
            review.GoalsSummary,
            review.Goals.Select(goal => goal.ToDto()).ToArray(),
            review.KeyPerformanceIndicators.Select(kpi => kpi.ToDto()).ToArray(),
            review.FeedbackCycles.Select(feedback => feedback.ToDto()).ToArray(),
            review.CompensationReview?.ToDto(),
            review.SubmittedAtUtc);
    }

    public static PerformanceReview ToEntity(this CreatePerformanceReviewRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        return new PerformanceReview
        {
            Id = Guid.NewGuid(),
            EmployeeId = request.EmployeeId,
            CycleName = request.CycleName.Trim(),
            PeriodStart = request.PeriodStart,
            PeriodEnd = request.PeriodEnd,
            OverallScore = request.OverallScore,
            ManagerComments = request.ManagerComments.Trim(),
            GoalsSummary = request.GoalsSummary.Trim(),
            Goals = request.Goals.Select(goal => goal.ToEntity()).ToArray(),
            KeyPerformanceIndicators = request.KeyPerformanceIndicators.Select(kpi => kpi.ToEntity()).ToArray(),
            FeedbackCycles = request.FeedbackCycles.Select(feedback => feedback.ToEntity()).ToArray(),
            CompensationReview = request.CompensationReview?.ToEntity(),
            SubmittedAtUtc = DateTime.UtcNow
        };
    }

    public static PerformanceReview ApplyUpdates(this UpdatePerformanceReviewRequest request, PerformanceReview existing)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(existing);

        return new PerformanceReview
        {
            Id = existing.Id,
            EmployeeId = existing.EmployeeId,
            CycleName = request.CycleName.Trim(),
            PeriodStart = request.PeriodStart,
            PeriodEnd = request.PeriodEnd,
            OverallScore = request.OverallScore,
            ManagerComments = request.ManagerComments.Trim(),
            GoalsSummary = request.GoalsSummary.Trim(),
            Goals = request.Goals.Select(goal => goal.ToEntity()).ToArray(),
            KeyPerformanceIndicators = request.KeyPerformanceIndicators.Select(kpi => kpi.ToEntity()).ToArray(),
            FeedbackCycles = request.FeedbackCycles.Select(feedback => feedback.ToEntity()).ToArray(),
            CompensationReview = request.CompensationReview?.ToEntity(),
            SubmittedAtUtc = request.SubmittedAtUtc
        };
    }

    private static PerformanceGoalDto ToDto(this PerformanceGoal goal)
    {
        return new PerformanceGoalDto(
            goal.Id,
            goal.Title,
            goal.Description,
            goal.Weight,
            goal.ParentGoalId,
            goal.Alignment,
            goal.Status);
    }

    private static PerformanceGoal ToEntity(this PerformanceGoalRequest goal)
    {
        ArgumentNullException.ThrowIfNull(goal);

        return new PerformanceGoal
        {
            Id = goal.Id ?? Guid.NewGuid(),
            Title = goal.Title.Trim(),
            Description = goal.Description.Trim(),
            Weight = goal.Weight,
            ParentGoalId = goal.ParentGoalId,
            Alignment = goal.Alignment.Trim(),
            Status = goal.Status.Trim()
        };
    }

    private static PerformanceKpiDto ToDto(this PerformanceKpi kpi)
    {
        return new PerformanceKpiDto(
            kpi.Id,
            kpi.Name,
            kpi.TargetValue,
            kpi.ActualValue,
            kpi.UnitOfMeasure,
            kpi.Status);
    }

    private static PerformanceKpi ToEntity(this PerformanceKpiRequest kpi)
    {
        ArgumentNullException.ThrowIfNull(kpi);

        return new PerformanceKpi
        {
            Id = kpi.Id ?? Guid.NewGuid(),
            Name = kpi.Name.Trim(),
            TargetValue = kpi.TargetValue,
            ActualValue = kpi.ActualValue,
            UnitOfMeasure = kpi.UnitOfMeasure.Trim(),
            Status = kpi.Status.Trim()
        };
    }

    private static PerformanceFeedbackDto ToDto(this PerformanceFeedback feedback)
    {
        return new PerformanceFeedbackDto(
            feedback.Id,
            feedback.FeedbackType,
            feedback.Comments,
            feedback.SubmittedBy,
            feedback.SubmittedAtUtc);
    }

    private static PerformanceFeedback ToEntity(this PerformanceFeedbackRequest feedback)
    {
        ArgumentNullException.ThrowIfNull(feedback);

        return new PerformanceFeedback
        {
            Id = feedback.Id ?? Guid.NewGuid(),
            FeedbackType = feedback.FeedbackType.Trim(),
            Comments = feedback.Comments.Trim(),
            SubmittedBy = feedback.SubmittedBy,
            SubmittedAtUtc = DateTime.SpecifyKind(feedback.SubmittedAtUtc, DateTimeKind.Utc)
        };
    }

    private static CompensationReviewDto ToDto(this CompensationReview review)
    {
        return new CompensationReviewDto(
            review.EffectiveDate,
            review.CurrentBaseSalary,
            review.ProposedBaseSalary,
            review.BonusRecommendation,
            review.Currency,
            review.Notes);
    }

    private static CompensationReview ToEntity(this CompensationReviewRequest review)
    {
        ArgumentNullException.ThrowIfNull(review);

        return new CompensationReview
        {
            EffectiveDate = review.EffectiveDate,
            CurrentBaseSalary = review.CurrentBaseSalary,
            ProposedBaseSalary = review.ProposedBaseSalary,
            BonusRecommendation = review.BonusRecommendation,
            Currency = review.Currency.Trim().ToUpperInvariant(),
            Notes = review.Notes.Trim()
        };
    }
}
