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
            SubmittedAtUtc = request.SubmittedAtUtc
        };
    }
}
