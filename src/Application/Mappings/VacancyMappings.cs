using System;
using System.Collections.Generic;
using System.Linq;
using HR.Application.DTOs;
using HR.Domain.Entities;

namespace HR.Application.Mappings;

/// <summary>
///     Mapping helpers for <see cref="Vacancy" /> aggregates.
/// </summary>
public static class VacancyMappings
{
    public static VacancyDto ToDto(this Vacancy vacancy)
    {
        ArgumentNullException.ThrowIfNull(vacancy);

        return new VacancyDto(
            vacancy.Id,
            vacancy.RequisitionId,
            vacancy.PublicTitle,
            vacancy.PublicDescription,
            vacancy.Location,
            vacancy.WorkMode,
            vacancy.EmploymentType,
            vacancy.SalaryVisible,
            vacancy.SalaryRangeText,
            vacancy.NumberOfPositions,
            vacancy.Department,
            vacancy.Responsibilities,
            vacancy.Requirements,
            vacancy.PostingChannels,
            vacancy.PipelineStages,
            vacancy.HiringTeam,
            vacancy.CreatedAtUtc,
            vacancy.PublishedAtUtc,
            vacancy.ClosedAtUtc,
            vacancy.Status,
            vacancy.ApplicationUrl);
    }

    public static Vacancy ToEntity(this CreateVacancyRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        return new Vacancy
        {
            Id = Guid.NewGuid(),
            RequisitionId = request.RequisitionId,
            PublicTitle = request.PublicTitle.Trim(),
            Department = request.Department.Trim(),
            Location = request.Location.Trim(),
            EmploymentType = request.EmploymentType.Trim(),
            WorkMode = request.WorkMode.Trim(),
            SalaryVisible = request.SalaryVisible,
            SalaryRangeText = request.SalaryRangeText.Trim(),
            NumberOfPositions = request.NumberOfPositions,
            PublicDescription = request.PublicDescription.Trim(),
            Responsibilities = Normalize(request.Responsibilities),
            Requirements = Normalize(request.Requirements),
            PostingChannels = Normalize(request.PostingChannels),
            PipelineStages = Normalize(request.PipelineStages),
            HiringTeam = Normalize(request.HiringTeam),
            CreatedAtUtc = DateTime.UtcNow,
            PublishedAtUtc = DateTime.UtcNow,
            Status = "Published",
            ApplicationUrl = request.ApplicationUrl.Trim()
        };
    }

    public static Vacancy ApplyUpdates(this UpdateVacancyRequest request, Vacancy existing)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(existing);

        var status = string.IsNullOrWhiteSpace(request.Status) ? existing.Status : request.Status.Trim();
        var closedAt = request.ClosedAtUtc ?? existing.ClosedAtUtc;
        var publishedAt = request.PublishedAtUtc ?? existing.PublishedAtUtc ?? existing.CreatedAtUtc;

        return new Vacancy
        {
            Id = existing.Id,
            RequisitionId = request.RequisitionId,
            PublicTitle = request.PublicTitle.Trim(),
            Department = request.Department.Trim(),
            Location = request.Location.Trim(),
            EmploymentType = request.EmploymentType.Trim(),
            WorkMode = request.WorkMode.Trim(),
            SalaryVisible = request.SalaryVisible,
            SalaryRangeText = request.SalaryRangeText.Trim(),
            NumberOfPositions = request.NumberOfPositions,
            PublicDescription = request.PublicDescription.Trim(),
            Responsibilities = Normalize(request.Responsibilities),
            Requirements = Normalize(request.Requirements),
            PostingChannels = Normalize(request.PostingChannels),
            PipelineStages = Normalize(request.PipelineStages),
            HiringTeam = Normalize(request.HiringTeam),
            CreatedAtUtc = existing.CreatedAtUtc,
            PublishedAtUtc = publishedAt,
            ClosedAtUtc = closedAt,
            Status = status,
            ApplicationUrl = request.ApplicationUrl.Trim()
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
