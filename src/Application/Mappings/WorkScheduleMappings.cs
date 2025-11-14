using HR.Application.DTOs;
using HR.Domain.Entities;

namespace HR.Application.Mappings;

/// <summary>
///     Mapping helpers for work schedule aggregates.
/// </summary>
public static class WorkScheduleMappings
{
    public static WorkScheduleDto ToDto(this WorkSchedule schedule)
    {
        ArgumentNullException.ThrowIfNull(schedule);

        return new WorkScheduleDto(
            schedule.Id,
            schedule.Name,
            schedule.OrganizationId,
            schedule.DepartmentId,
            schedule.IsDefaultForOrganization,
            schedule.TimeZoneId,
            schedule.ShiftTemplates.Select(template => template.ToDto()).ToArray());
    }

    public static WorkSchedule ToEntity(this CreateWorkScheduleRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var entity = new WorkSchedule
        {
            Id = Guid.NewGuid(),
            Name = request.Name.Trim(),
            OrganizationId = request.OrganizationId,
            DepartmentId = request.DepartmentId,
            IsDefaultForOrganization = request.IsDefaultForOrganization,
            TimeZoneId = request.TimeZoneId.Trim()
        };

        foreach (var template in request.ShiftTemplates.ToDomainShiftTemplates(entity.Id))
        {
            entity.ShiftTemplates.Add(template);
        }

        return entity;
    }

    public static WorkSchedule ApplyUpdates(this UpdateWorkScheduleRequest request, WorkSchedule existing)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(existing);

        var entity = new WorkSchedule
        {
            Id = existing.Id,
            Name = request.Name.Trim(),
            OrganizationId = request.OrganizationId,
            DepartmentId = request.DepartmentId,
            IsDefaultForOrganization = request.IsDefaultForOrganization,
            TimeZoneId = request.TimeZoneId.Trim()
        };

        foreach (var template in request.ShiftTemplates.ToDomainShiftTemplates(existing.Id))
        {
            entity.ShiftTemplates.Add(template);
        }

        return entity;
    }

    private static ShiftTemplateDto ToDto(this ShiftTemplate template)
    {
        return new ShiftTemplateDto(
            template.Id,
            template.DayOfWeek,
            template.StartTime,
            template.EndTime,
            template.BreakMinutes,
            template.GracePeriodMinutes,
            template.MinimumOvertimeMinutes);
    }

    private static IReadOnlyCollection<ShiftTemplate> ToDomainShiftTemplates(
        this IEnumerable<ShiftTemplateRequest>? requests,
        Guid workScheduleId)
    {
        if (requests is null)
        {
            return Array.Empty<ShiftTemplate>();
        }

        return requests
            .Select(request => new ShiftTemplate
            {
                Id = Guid.NewGuid(),
                WorkScheduleId = workScheduleId,
                DayOfWeek = request.DayOfWeek,
                StartTime = request.StartTime,
                EndTime = request.EndTime,
                BreakMinutes = request.BreakMinutes,
                GracePeriodMinutes = request.GracePeriodMinutes,
                MinimumOvertimeMinutes = request.MinimumOvertimeMinutes
            })
            .ToArray();
    }
}
