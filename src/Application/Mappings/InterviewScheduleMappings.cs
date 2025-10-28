using HR.Application.DTOs;
using HR.Domain.Entities;

namespace HR.Application.Mappings;

/// <summary>
///     Mapping helpers for <see cref="InterviewSchedule" /> entities.
/// </summary>
public static class InterviewScheduleMappings
{
    public static InterviewScheduleDto ToDto(this InterviewSchedule schedule)
    {
        ArgumentNullException.ThrowIfNull(schedule);

        return new InterviewScheduleDto(
            schedule.Id,
            schedule.CandidateId,
            schedule.VacancyId,
            schedule.Stage,
            schedule.ScheduledAtUtc,
            (int)Math.Round(schedule.Duration.TotalMinutes),
            schedule.Mode,
            schedule.Location,
            schedule.MeetingLink,
            schedule.Interviewers,
            schedule.Status,
            schedule.Notes);
    }

    public static InterviewSchedule ToEntity(this ScheduleInterviewRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        return new InterviewSchedule
        {
            Id = Guid.NewGuid(),
            CandidateId = request.CandidateId,
            VacancyId = request.VacancyId,
            Stage = request.Stage.Trim(),
            ScheduledAtUtc = request.ScheduledAtUtc,
            Duration = TimeSpan.FromMinutes(request.DurationMinutes),
            Mode = request.Mode.Trim(),
            Location = request.Location.Trim(),
            MeetingLink = request.MeetingLink.Trim(),
            Interviewers = Normalize(request.Interviewers),
            Status = "Scheduled",
            Notes = request.Notes.Trim()
        };
    }

    public static InterviewSchedule ApplyUpdates(this UpdateInterviewScheduleRequest request, InterviewSchedule existing)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(existing);

        var status = string.IsNullOrWhiteSpace(request.Status) ? existing.Status : request.Status.Trim();

        return new InterviewSchedule
        {
            Id = existing.Id,
            CandidateId = existing.CandidateId,
            VacancyId = existing.VacancyId,
            Stage = request.Stage.Trim(),
            ScheduledAtUtc = request.ScheduledAtUtc,
            Duration = TimeSpan.FromMinutes(request.DurationMinutes),
            Mode = request.Mode.Trim(),
            Location = request.Location.Trim(),
            MeetingLink = request.MeetingLink.Trim(),
            Interviewers = Normalize(request.Interviewers),
            Status = status,
            Notes = request.Notes.Trim()
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
