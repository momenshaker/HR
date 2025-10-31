namespace HR.Application.Abstractions.Services;

/// <summary>
///     Hook for weekend/holiday handling and workday calculations.
/// </summary>
public interface IWorkdayCalendar
{
    bool IsWorkday(DateOnly date);
}

