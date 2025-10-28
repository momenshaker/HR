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
            vacancy.Title,
            vacancy.Department,
            vacancy.Location,
            vacancy.EmploymentType,
            vacancy.Description,
            vacancy.Responsibilities,
            vacancy.Requirements,
            vacancy.PipelineStages,
            vacancy.HiringTeam,
            vacancy.PostedAtUtc,
            vacancy.ClosingAtUtc,
            vacancy.Status,
            vacancy.ApplicationUrl);
    }

    public static Vacancy ToEntity(this CreateVacancyRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        return new Vacancy
        {
            Id = Guid.NewGuid(),
            Title = request.Title.Trim(),
            Department = request.Department.Trim(),
            Location = request.Location.Trim(),
            EmploymentType = request.EmploymentType.Trim(),
            Description = request.Description.Trim(),
            Responsibilities = Normalize(request.Responsibilities),
            Requirements = Normalize(request.Requirements),
            PipelineStages = Normalize(request.PipelineStages),
            HiringTeam = Normalize(request.HiringTeam),
            PostedAtUtc = DateTime.UtcNow,
            ClosingAtUtc = request.ClosingAtUtc,
            Status = "Open",
            ApplicationUrl = request.ApplicationUrl.Trim()
        };
    }

    public static Vacancy ApplyUpdates(this UpdateVacancyRequest request, Vacancy existing)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(existing);

        var status = string.IsNullOrWhiteSpace(request.Status) ? existing.Status : request.Status.Trim();
        var closingAt = request.ClosingAtUtc ?? existing.ClosingAtUtc;

        return new Vacancy
        {
            Id = existing.Id,
            Title = request.Title.Trim(),
            Department = request.Department.Trim(),
            Location = request.Location.Trim(),
            EmploymentType = request.EmploymentType.Trim(),
            Description = request.Description.Trim(),
            Responsibilities = Normalize(request.Responsibilities),
            Requirements = Normalize(request.Requirements),
            PipelineStages = Normalize(request.PipelineStages),
            HiringTeam = Normalize(request.HiringTeam),
            PostedAtUtc = existing.PostedAtUtc,
            ClosingAtUtc = closingAt,
            Status = status,
            ApplicationUrl = request.ApplicationUrl.Trim()
        };
    }

    private static IReadOnlyCollection<string> Normalize(IReadOnlyCollection<string>? values)
    {
        if (values is null || values.Count == 0)
        {
            return Array.Empty<string>();
        }

        return values
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .Select(value => value.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }
}
