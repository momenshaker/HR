using HR.Application.DTOs;
using HR.Domain.Entities;

namespace HR.Application.Mappings;

/// <summary>
///     Mapping helpers for <see cref="LeaveRequest" /> entities.
/// </summary>
public static class LeaveRequestMappings
{
    public static LeaveRequestDto ToDto(this LeaveRequest leave)
    {
        ArgumentNullException.ThrowIfNull(leave);

        return new LeaveRequestDto(
            leave.Id,
            leave.EmployeeId,
            leave.LeaveType,
            leave.StartDate,
            leave.EndDate,
            leave.Status,
            leave.ApproverId,
            leave.Reason,
            leave.RequestedAtUtc,
            leave.DecisionAtUtc);
    }

    public static LeaveRequest ToEntity(this CreateLeaveRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        return new LeaveRequest
        {
            Id = Guid.NewGuid(),
            EmployeeId = request.EmployeeId,
            LeaveType = request.LeaveType.Trim(),
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            Reason = request.Reason.Trim(),
            Status = "Pending",
            RequestedAtUtc = DateTime.UtcNow
        };
    }

    public static LeaveRequest ApplyUpdates(this UpdateLeaveRequest request, LeaveRequest existing)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(existing);

        return new LeaveRequest
        {
            Id = existing.Id,
            EmployeeId = existing.EmployeeId,
            LeaveType = request.LeaveType.Trim(),
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            Reason = request.Reason.Trim(),
            Status = request.Status.Trim(),
            ApproverId = request.ApproverId,
            RequestedAtUtc = existing.RequestedAtUtc,
            DecisionAtUtc = request.DecisionAtUtc
        };
    }
}
