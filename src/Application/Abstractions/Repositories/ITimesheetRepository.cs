using HR.Domain.Entities;

namespace HR.Application.Abstractions.Repositories;

public interface ITimesheetRepository
{
    Task<Timesheet?> GetByIdAsync(Guid timesheetId, CancellationToken cancellationToken = default);

    Task<Timesheet?> GetByEmployeeWeekAsync(Guid employeeId, DateOnly weekStartUtc, CancellationToken cancellationToken = default);

    Task<Timesheet> AddAsync(Timesheet timesheet, CancellationToken cancellationToken = default);

    Task<Timesheet?> UpdateAsync(Timesheet timesheet, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<Timesheet>> GetApprovalsAsync(
        Guid managerId,
        TimesheetStatus? status,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);
}

