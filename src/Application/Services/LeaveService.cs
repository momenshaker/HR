using HR.Application.Abstractions.Repositories;
using HR.Application.Abstractions.Services;
using HR.Application.DTOs;
using HR.Application.Mappings;
using HR.Domain.Entities;

namespace HR.Application.Services;

/// <summary>
///     Leave management service implementing preview/balances and approval workflow.
/// </summary>
public sealed class LeaveService(
    ILeaveRequestRepository leaveRequestRepository,
    ILeaveTypeRepository leaveTypeRepository,
    ILeaveBalanceRepository leaveBalanceRepository,
    IWorkdayCalendar calendar) : ILeaveService
{
    private readonly ILeaveRequestRepository _leaveRequests = leaveRequestRepository;
    private readonly ILeaveTypeRepository _leaveTypes = leaveTypeRepository;
    private readonly ILeaveBalanceRepository _leaveBalances = leaveBalanceRepository;
    private readonly IWorkdayCalendar _calendar = calendar;

    public async Task<IReadOnlyCollection<LeaveTypeDto>> GetLeaveTypesAsync(CancellationToken cancellationToken = default)
    {
        var types = await _leaveTypes.GetAllAsync(cancellationToken).ConfigureAwait(false);
        return types.Select(t => t.ToDto()).ToArray();
    }

    public async Task<IReadOnlyCollection<LeaveBalanceDto>> GetBalancesAsync(Guid employeeId, int year, CancellationToken cancellationToken = default)
    {
        var balances = await _leaveBalances.GetByEmployeeYearAsync(employeeId, year, cancellationToken).ConfigureAwait(false);
        var requests = await _leaveRequests.GetAllAsync(cancellationToken).ConfigureAwait(false);

        var reservedByType = requests
            .Where(r => r.EmployeeId == employeeId && r.Status.Equals("Pending", StringComparison.OrdinalIgnoreCase))
            .GroupBy(r => new { Year = r.StartDate.Year, r.LeaveType })
            .ToDictionary(g => (g.Key.Year, g.Key.LeaveType), g => g.Sum(r => CalculateDuration(r.StartDate, r.EndDate)));

        var typeMap = (await _leaveTypes.GetAllAsync(cancellationToken).ConfigureAwait(false))
            .ToDictionary(t => t.Id, t => t);

        var result = new List<LeaveBalanceDto>(balances.Count);
        foreach (var b in balances)
        {
            // Find reserved by code: we need mapping from type id->code
            if (!typeMap.TryGetValue(b.LeaveTypeId, out var lt))
            {
                continue;
            }

            var reserved = reservedByType.TryGetValue((b.Year, lt.Code), out var res) ? res : 0m;
            result.Add(b.ToDto(reserved));
        }

        return result;
    }

    public async Task<LeavePreviewDto> PreviewAsync(Guid employeeId, Guid leaveTypeId, DateOnly startDate, DateOnly endDate, CancellationToken cancellationToken = default)
    {
        if (endDate < startDate)
            throw new ArgumentException("End date must be on or after start date.");

        var duration = CalculateDuration(startDate, endDate);
        var year = startDate.Year;
        var balance = await _leaveBalances.GetAsync(employeeId, leaveTypeId, year, cancellationToken).ConfigureAwait(false)
                      ?? new LeaveBalance { EmployeeId = employeeId, LeaveTypeId = leaveTypeId, Year = year };

        var type = await _leaveTypes.GetByIdAsync(leaveTypeId, cancellationToken).ConfigureAwait(false)
                   ?? throw new InvalidOperationException("Leave type not found.");

        var currentAvailable = (balance.Opening + balance.Accrued + balance.CarriedOver) - balance.Taken;

        var reserved = await GetReservedAsync(employeeId, type.Code, year, cancellationToken).ConfigureAwait(false);

        var availableAfter = currentAvailable - reserved - duration;

        return new LeavePreviewDto(duration, currentAvailable, reserved, availableAfter);
    }

    public async Task<LeaveRequestDto> SubmitAsync(SubmitLeaveRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        if (request.EndDate < request.StartDate)
            throw new ArgumentException("End date must be on or after start date.");

        var leaveType = await _leaveTypes.GetByIdAsync(request.LeaveTypeId, cancellationToken).ConfigureAwait(false)
                        ?? throw new InvalidOperationException("Leave type not found.");

        var overlapError = await FindOverlapAsync(request.EmployeeId, request.StartDate, request.EndDate, cancellationToken).ConfigureAwait(false);
        if (overlapError is not null)
        {
            throw new InvalidOperationException(overlapError);
        }

        var year = request.StartDate.Year;
        var preview = await PreviewAsync(request.EmployeeId, request.LeaveTypeId, request.StartDate, request.EndDate, cancellationToken).ConfigureAwait(false);
        if (preview.AvailableAfter < 0)
        {
            throw new InvalidOperationException("Insufficient leave balance.");
        }

        var entity = new LeaveRequest
        {
            Id = Guid.NewGuid(),
            EmployeeId = request.EmployeeId,
            LeaveType = leaveType.Code,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            Reason = request.Reason?.Trim() ?? string.Empty,
            Status = leaveType.RequiresApproval ? "Pending" : "Approved",
            ApproverId = null,
            RequestedAtUtc = DateTime.UtcNow,
            DecisionAtUtc = leaveType.RequiresApproval ? null : DateTime.UtcNow
        };

        var created = await _leaveRequests.AddAsync(entity, cancellationToken).ConfigureAwait(false);

        // If auto-approved (no approval required), immediately update Taken balance.
        if (!leaveType.RequiresApproval)
        {
            var duration = CalculateDuration(entity.StartDate, entity.EndDate);
            var balance = await _leaveBalances.GetAsync(entity.EmployeeId, request.LeaveTypeId, year, cancellationToken).ConfigureAwait(false)
                          ?? new LeaveBalance
                          {
                              EmployeeId = entity.EmployeeId,
                              LeaveTypeId = request.LeaveTypeId,
                              Year = year,
                              Opening = 0,
                              Accrued = leaveType.AnnualAllowanceDays,
                              Taken = 0,
                              CarriedOver = 0
                          };

            var newTaken = balance.Taken + duration;
            await _leaveBalances.UpsertAsync(new LeaveBalance
                {
                    EmployeeId = balance.EmployeeId,
                    LeaveTypeId = balance.LeaveTypeId,
                    Year = balance.Year,
                    Opening = balance.Opening,
                    Accrued = balance.Accrued,
                    Taken = newTaken,
                    CarriedOver = balance.CarriedOver,
                    RowVersion = balance.RowVersion
                }, cancellationToken)
                .ConfigureAwait(false);
        }

        return created.ToDto();
    }

    public async Task<LeaveRequestDto> ApproveAsync(Guid requestId, Guid managerId, CancellationToken cancellationToken = default)
    {
        var request = await _leaveRequests.GetByIdAsync(requestId, cancellationToken).ConfigureAwait(false)
                      ?? throw new InvalidOperationException("Leave request not found.");
        if (!string.Equals(request.Status, "Pending", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Only pending requests can be approved.");
        }

        // Confirm deduction
        var year = request.StartDate.Year;
        var leaveType = (await _leaveTypes.GetAllAsync(cancellationToken).ConfigureAwait(false))
            .First(t => t.Code.Equals(request.LeaveType, StringComparison.OrdinalIgnoreCase));

        var duration = CalculateDuration(request.StartDate, request.EndDate);
        var balance = await _leaveBalances.GetAsync(request.EmployeeId, leaveType.Id, year, cancellationToken).ConfigureAwait(false)
                      ?? new LeaveBalance
                      {
                          EmployeeId = request.EmployeeId,
                          LeaveTypeId = leaveType.Id,
                          Year = year,
                          Opening = 0,
                          Accrued = leaveType.AnnualAllowanceDays,
                          Taken = 0,
                          CarriedOver = 0
                      };

        var newTaken = balance.Taken + duration;
        await _leaveBalances.UpsertAsync(new LeaveBalance
            {
                EmployeeId = balance.EmployeeId,
                LeaveTypeId = balance.LeaveTypeId,
                Year = balance.Year,
                Opening = balance.Opening,
                Accrued = balance.Accrued,
                Taken = newTaken,
                CarriedOver = balance.CarriedOver,
                RowVersion = balance.RowVersion
            }, cancellationToken).ConfigureAwait(false);

        var updated = new LeaveRequest
        {
            Id = request.Id,
            EmployeeId = request.EmployeeId,
            LeaveType = request.LeaveType,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            Status = "Approved",
            ApproverId = managerId,
            Reason = request.Reason,
            RequestedAtUtc = request.RequestedAtUtc,
            DecisionAtUtc = DateTime.UtcNow
        };

        var persisted = await _leaveRequests.UpdateAsync(updated, cancellationToken).ConfigureAwait(false)
                        ?? throw new InvalidOperationException("Failed to update leave request.");

        return persisted.ToDto();
    }

    public async Task<LeaveRequestDto> RejectAsync(Guid requestId, Guid managerId, string reason, CancellationToken cancellationToken = default)
    {
        var request = await _leaveRequests.GetByIdAsync(requestId, cancellationToken).ConfigureAwait(false)
                      ?? throw new InvalidOperationException("Leave request not found.");
        if (!string.Equals(request.Status, "Pending", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Only pending requests can be rejected.");
        }

        var updated = new LeaveRequest
        {
            Id = request.Id,
            EmployeeId = request.EmployeeId,
            LeaveType = request.LeaveType,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            Status = "Rejected",
            ApproverId = managerId,
            Reason = string.IsNullOrWhiteSpace(reason) ? request.Reason : reason.Trim(),
            RequestedAtUtc = request.RequestedAtUtc,
            DecisionAtUtc = DateTime.UtcNow
        };

        var persisted = await _leaveRequests.UpdateAsync(updated, cancellationToken).ConfigureAwait(false)
                        ?? throw new InvalidOperationException("Failed to update leave request.");
        return persisted.ToDto();
    }

    public async Task<LeaveRequestDto> CancelAsync(Guid requestId, Guid employeeId, CancellationToken cancellationToken = default)
    {
        var request = await _leaveRequests.GetByIdAsync(requestId, cancellationToken).ConfigureAwait(false)
                      ?? throw new InvalidOperationException("Leave request not found.");
        if (request.EmployeeId != employeeId)
        {
            throw new InvalidOperationException("Only the owner can cancel the request.");
        }

        if (!string.Equals(request.Status, "Pending", StringComparison.OrdinalIgnoreCase) &&
            !string.Equals(request.Status, "Approved", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Only pending or approved requests can be cancelled.");
        }

        // Only future requests can be cancelled
        if (request.StartDate <= DateOnly.FromDateTime(DateTime.UtcNow.Date))
        {
            throw new InvalidOperationException("Only future leave can be cancelled.");
        }

        // If previously approved, revert taken days
        if (string.Equals(request.Status, "Approved", StringComparison.OrdinalIgnoreCase))
        {
            var leaveType = (await _leaveTypes.GetAllAsync(cancellationToken).ConfigureAwait(false))
                .First(t => t.Code.Equals(request.LeaveType, StringComparison.OrdinalIgnoreCase));

            var year = request.StartDate.Year;
            var duration = CalculateDuration(request.StartDate, request.EndDate);
            var balance = await _leaveBalances.GetAsync(request.EmployeeId, leaveType.Id, year, cancellationToken).ConfigureAwait(false)
                          ?? throw new InvalidOperationException("Balance not found.");

            var newTaken = Math.Max(0, balance.Taken - duration);
            await _leaveBalances.UpsertAsync(new LeaveBalance
                {
                    EmployeeId = balance.EmployeeId,
                    LeaveTypeId = balance.LeaveTypeId,
                    Year = balance.Year,
                    Opening = balance.Opening,
                    Accrued = balance.Accrued,
                    Taken = newTaken,
                    CarriedOver = balance.CarriedOver,
                    RowVersion = balance.RowVersion
                }, cancellationToken).ConfigureAwait(false);
        }

        var updated = new LeaveRequest
        {
            Id = request.Id,
            EmployeeId = request.EmployeeId,
            LeaveType = request.LeaveType,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            Status = "Cancelled",
            ApproverId = request.ApproverId,
            Reason = request.Reason,
            RequestedAtUtc = request.RequestedAtUtc,
            DecisionAtUtc = DateTime.UtcNow
        };

        var persisted = await _leaveRequests.UpdateAsync(updated, cancellationToken).ConfigureAwait(false)
                        ?? throw new InvalidOperationException("Failed to update leave request.");
        return persisted.ToDto();
    }

    public async Task<PagedLeaveRequestsDto> GetRequestsAsync(Guid? employeeId, Guid? managerId, string? status, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 200);

        var requests = await _leaveRequests.GetAllAsync(cancellationToken).ConfigureAwait(false);

        var filtered = requests.AsEnumerable();
        if (employeeId is not null)
            filtered = filtered.Where(r => r.EmployeeId == employeeId);
        if (managerId is not null)
            filtered = filtered.Where(r => r.ApproverId == managerId);
        if (!string.IsNullOrWhiteSpace(status))
            filtered = filtered.Where(r => string.Equals(r.Status, status, StringComparison.OrdinalIgnoreCase));

        var total = filtered.Count();
        var items = filtered
            .OrderByDescending(r => r.RequestedAtUtc)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(r => r.ToDto())
            .ToArray();

        return new PagedLeaveRequestsDto(page, pageSize, total, items);
    }

    private async Task<string?> FindOverlapAsync(Guid employeeId, DateOnly start, DateOnly end, CancellationToken ct)
    {
        var requests = await _leaveRequests.GetAllAsync(ct).ConfigureAwait(false);
        var overlapping = requests
            .Where(r => r.EmployeeId == employeeId)
            .Where(r => r.Status.Equals("Pending", StringComparison.OrdinalIgnoreCase) || r.Status.Equals("Approved", StringComparison.OrdinalIgnoreCase))
            .Where(r => DateRangesOverlap(start, end, r.StartDate, r.EndDate))
            .FirstOrDefault();

        return overlapping is null ? null : "Overlapping leave request exists.";
    }

    private async Task<decimal> GetReservedAsync(Guid employeeId, string leaveTypeCode, int year, CancellationToken ct)
    {
        var requests = await _leaveRequests.GetAllAsync(ct).ConfigureAwait(false);
        return requests
            .Where(r => r.EmployeeId == employeeId && r.Status.Equals("Pending", StringComparison.OrdinalIgnoreCase))
            .Where(r => r.StartDate.Year == year)
            .Where(r => string.Equals(r.LeaveType, leaveTypeCode, StringComparison.OrdinalIgnoreCase))
            .Sum(r => CalculateDuration(r.StartDate, r.EndDate));
    }

    private bool DateRangesOverlap(DateOnly aStart, DateOnly aEnd, DateOnly bStart, DateOnly bEnd)
    {
        return aStart <= bEnd && bStart <= aEnd;
    }

    private decimal CalculateDuration(DateOnly start, DateOnly end)
    {
        var days = 0m;
        var cursor = start;
        while (cursor <= end)
        {
            if (_calendar.IsWorkday(cursor))
                days += 1m;
            cursor = cursor.AddDays(1);
        }
        return days;
    }
}

