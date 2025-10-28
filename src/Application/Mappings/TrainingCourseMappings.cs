using HR.Application.DTOs;
using HR.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HR.Application.Mappings;

/// <summary>
///     Mapping helpers for <see cref="TrainingCourse" /> entities.
/// </summary>
public static class TrainingCourseMappings
{
    public static TrainingCourseDto ToDto(this TrainingCourse course)
    {
        ArgumentNullException.ThrowIfNull(course);

        return new TrainingCourseDto(
            course.Id,
            course.Title,
            course.Category,
            course.Description,
            course.Instructor,
            course.StartDate,
            course.EndDate,
            course.Capacity,
            course.DeliveryMode,
            course.CompetencyCodes,
            course.SkillLevel,
            course.OffersCertification,
            course.CertificationCriteria,
            course.DurationHours);
    }

    public static TrainingCourse ToEntity(this CreateTrainingCourseRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var title = request.Title?.Trim() ?? throw new ArgumentException("Title is required.", nameof(request.Title));

        return new TrainingCourse
        {
            Id = Guid.NewGuid(),
            Title = title,
            Category = request.Category?.Trim() ?? string.Empty,
            Description = request.Description?.Trim() ?? string.Empty,
            Instructor = request.Instructor?.Trim() ?? string.Empty,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            Capacity = request.Capacity,
            DeliveryMode = request.DeliveryMode?.Trim() ?? string.Empty,
            CompetencyCodes = NormalizeCompetencyCodes(request.CompetencyCodes),
            SkillLevel = request.SkillLevel?.Trim() ?? string.Empty,
            OffersCertification = request.OffersCertification,
            CertificationCriteria = request.CertificationCriteria?.Trim() ?? string.Empty,
            DurationHours = request.DurationHours
        };
    }

    public static TrainingCourse ApplyUpdates(this UpdateTrainingCourseRequest request, TrainingCourse existing)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(existing);

        var title = request.Title?.Trim() ?? throw new ArgumentException("Title is required.", nameof(request.Title));

        return new TrainingCourse
        {
            Id = existing.Id,
            Title = title,
            Category = request.Category?.Trim() ?? string.Empty,
            Description = request.Description?.Trim() ?? string.Empty,
            Instructor = request.Instructor?.Trim() ?? string.Empty,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            Capacity = request.Capacity,
            DeliveryMode = request.DeliveryMode?.Trim() ?? string.Empty,
            CompetencyCodes = NormalizeCompetencyCodes(request.CompetencyCodes),
            SkillLevel = request.SkillLevel?.Trim() ?? string.Empty,
            OffersCertification = request.OffersCertification,
            CertificationCriteria = request.CertificationCriteria?.Trim() ?? string.Empty,
            DurationHours = request.DurationHours
        };
    }

    private static List<string> NormalizeCompetencyCodes(IEnumerable<string> codes)
    {
        if (codes is null)
        {
            return new List<string>();
        }

        return codes
            .Where(code => !string.IsNullOrWhiteSpace(code))
            .Select(code => code.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(code => code, StringComparer.OrdinalIgnoreCase)
            .ToList();
    }
}
