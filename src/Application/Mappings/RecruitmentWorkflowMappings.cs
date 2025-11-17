using System;
using System.Collections.Generic;
using System.Linq;
using HR.Application.DTOs;
using HR.Domain.Entities;

namespace HR.Application.Mappings;

/// <summary>
///     Mapping helpers for the recruitment workflow domain models.
/// </summary>
public static class RecruitmentWorkflowMappings
{
    public static JobRequisitionDto ToDto(this JobRequisition requisition)
    {
        ArgumentNullException.ThrowIfNull(requisition);

        return new JobRequisitionDto(
            requisition.Id,
            requisition.Title,
            requisition.DepartmentId,
            requisition.HiringManagerId,
            requisition.RequestedById,
            requisition.NumberOfPositions,
            requisition.EmploymentType,
            requisition.Location,
            requisition.BudgetedSalaryMin,
            requisition.BudgetedSalaryMax,
            requisition.Description,
            requisition.Status,
            requisition.ApprovalWorkflow);
    }

    public static JobRequisition ToEntity(this CreateJobRequisitionRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        return new JobRequisition
        {
            Id = Guid.NewGuid(),
            Title = request.Title.Trim(),
            DepartmentId = request.DepartmentId,
            HiringManagerId = request.HiringManagerId,
            RequestedById = request.RequestedById,
            NumberOfPositions = request.NumberOfPositions,
            EmploymentType = request.EmploymentType.Trim(),
            Location = request.Location.Trim(),
            BudgetedSalaryMin = request.BudgetedSalaryMin,
            BudgetedSalaryMax = request.BudgetedSalaryMax,
            Description = request.Description.Trim(),
            Status = string.IsNullOrWhiteSpace(request.Status) ? "Draft" : request.Status.Trim(),
            ApprovalWorkflow = Normalize(request.ApprovalWorkflow)
        };
    }

    public static JobApplicationDto ToDto(this JobApplication application)
    {
        ArgumentNullException.ThrowIfNull(application);

        return new JobApplicationDto(
            application.Id,
            application.CandidateId,
            application.JobPostingId,
            application.AppliedDate,
            application.CurrentStage,
            application.Status,
            application.Source,
            application.CVUrl,
            application.CoverLetter,
            application.ExpectedSalary,
            application.NoticePeriod,
            application.OverallScore);
    }

    public static JobApplication ToEntity(this CreateJobApplicationRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        return new JobApplication
        {
            Id = Guid.NewGuid(),
            CandidateId = request.CandidateId,
            JobPostingId = request.JobPostingId,
            AppliedDate = request.AppliedDate == default ? DateTime.UtcNow : request.AppliedDate,
            CurrentStage = request.CurrentStage.Trim(),
            Status = string.IsNullOrWhiteSpace(request.Status) ? "Active" : request.Status.Trim(),
            Source = request.Source.Trim(),
            CVUrl = request.CVUrl.Trim(),
            CoverLetter = request.CoverLetter.Trim(),
            ExpectedSalary = request.ExpectedSalary,
            NoticePeriod = request.NoticePeriod.Trim(),
            OverallScore = null
        };
    }

    public static PipelineStageDto ToDto(this PipelineStage stage)
    {
        ArgumentNullException.ThrowIfNull(stage);

        return new PipelineStageDto(
            stage.Id,
            stage.JobPostingId,
            stage.IsDefault,
            stage.Name,
            stage.Order,
            stage.IsFinalStage,
            stage.AutoEmailTemplateOnEnter);
    }

    public static PipelineStage ToEntity(this CreatePipelineStageRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        return new PipelineStage
        {
            Id = Guid.NewGuid(),
            JobPostingId = request.JobPostingId,
            IsDefault = request.IsDefault,
            Name = request.Name.Trim(),
            Order = request.Order,
            IsFinalStage = request.IsFinalStage,
            AutoEmailTemplateOnEnter = request.AutoEmailTemplateOnEnter.Trim()
        };
    }

    public static ApplicationStageHistoryDto ToDto(this ApplicationStageHistory history)
    {
        ArgumentNullException.ThrowIfNull(history);

        return new ApplicationStageHistoryDto(
            history.Id,
            history.ApplicationId,
            history.FromStage,
            history.ToStage,
            history.ChangedBy,
            history.ChangedAt,
            history.Reason);
    }

    public static ApplicationStageHistory ToEntity(this RecordApplicationStageChangeRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        return new ApplicationStageHistory
        {
            Id = Guid.NewGuid(),
            ApplicationId = request.ApplicationId,
            FromStage = request.FromStage.Trim(),
            ToStage = request.ToStage.Trim(),
            ChangedBy = request.ChangedBy,
            ChangedAt = request.ChangedAt,
            Reason = request.Reason.Trim()
        };
    }

    public static InterviewFeedbackDto ToDto(this InterviewFeedback feedback)
    {
        ArgumentNullException.ThrowIfNull(feedback);

        return new InterviewFeedbackDto(
            feedback.Id,
            feedback.InterviewId,
            feedback.ApplicationId,
            feedback.StageId,
            feedback.ReviewerId,
            feedback.RatingOverall,
            feedback.RatingTechnical,
            feedback.RatingCultureFit,
            feedback.Strengths,
            feedback.Weaknesses,
            feedback.Recommendation,
            feedback.CreatedAt);
    }

    public static InterviewFeedback ToEntity(this SubmitInterviewFeedbackRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        return new InterviewFeedback
        {
            Id = Guid.NewGuid(),
            InterviewId = request.InterviewId,
            ApplicationId = request.ApplicationId,
            StageId = request.StageId,
            ReviewerId = request.ReviewerId,
            RatingOverall = request.RatingOverall,
            RatingTechnical = request.RatingTechnical,
            RatingCultureFit = request.RatingCultureFit,
            Strengths = request.Strengths.Trim(),
            Weaknesses = request.Weaknesses.Trim(),
            Recommendation = request.Recommendation.Trim(),
            CreatedAt = DateTime.UtcNow
        };
    }

    public static OfferDto ToDto(this Offer offer)
    {
        ArgumentNullException.ThrowIfNull(offer);

        return new OfferDto(
            offer.Id,
            offer.ApplicationId,
            offer.PositionTitle,
            offer.EmploymentType,
            offer.ProposedSalary,
            offer.Currency,
            offer.StartDate,
            offer.ProbationPeriodMonths,
            offer.Status,
            offer.OfferDocumentUrl,
            offer.Comments);
    }

    public static Offer ToEntity(this CreateOfferRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        return new Offer
        {
            Id = Guid.NewGuid(),
            ApplicationId = request.ApplicationId,
            PositionTitle = request.PositionTitle.Trim(),
            EmploymentType = request.EmploymentType.Trim(),
            ProposedSalary = request.ProposedSalary,
            Currency = request.Currency.Trim(),
            StartDate = request.StartDate,
            ProbationPeriodMonths = request.ProbationPeriodMonths,
            Status = string.IsNullOrWhiteSpace(request.Status) ? "Draft" : request.Status.Trim(),
            OfferDocumentUrl = request.OfferDocumentUrl.Trim(),
            Comments = request.Comments.Trim()
        };
    }

    private static List<string> Normalize(IReadOnlyCollection<string>? values)
    {
        if (values is null || values.Count == 0)
        {
            return new List<string>();
        }

        return values
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .Select(value => value.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
    }
}
