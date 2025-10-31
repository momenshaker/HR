using HR.Application.DTOs;
using HR.Domain.Entities;

namespace HR.Application.Abstractions.Services;

public interface ITimesheetService
{
    Task<TimesheetDto> GetWeekAsync(Guid employeeId, DateOnly weekStartUtc, CancellationToken cancellationToken = default);

    Task<TimesheetEntryDto> UpsertEntryAsync(Guid timesheetId, UpsertTimesheetEntryRequest request, CancellationToken cancellationToken = default);

    Task<TimesheetDto?> SubmitAsync(Guid timesheetId, CancellationToken cancellationToken = default);

    Task<TimesheetDto?> ApproveAsync(Guid timesheetId, Guid managerId, string? notes, CancellationToken cancellationToken = default);

    Task<TimesheetDto?> RejectAsync(Guid timesheetId, Guid managerId, string reason, CancellationToken cancellationToken = default);

    Task<PaginatedResponse<TimesheetDto>> GetApprovalsAsync(Guid managerId, TimesheetStatus? status, int page, int pageSize, CancellationToken cancellationToken = default);
}

