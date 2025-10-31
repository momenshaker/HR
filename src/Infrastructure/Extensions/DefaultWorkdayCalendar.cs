using HR.Application.Abstractions.Services;

namespace HR.Infrastructure.Extensions;

internal sealed class DefaultWorkdayCalendar : IWorkdayCalendar
{
    public bool IsWorkday(DateOnly date)
    {
        var day = date.DayOfWeek;
        // Hook for holidays can be added here later
        return day != DayOfWeek.Saturday && day != DayOfWeek.Sunday;
    }
}

