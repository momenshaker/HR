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
            leave.LeaveTypeId,
            leave.LeaveType,
            leave.StartDate,
            leave.EndDate,
            leave.NumberOfDays,
            leave.Status,
            leave.ApproverId,
            leave.Reason,
            leave.AttachmentPath,
            leave.SubmittedAtUtc,
            leave.ApprovedAtUtc,
            leave.RejectedAtUtc,
            leave.CancelledAtUtc);
    }

    public static LeaveRequest ToEntity(this CreateLeaveRequest request, LeaveType leaveType)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(leaveType);

        var reason = request.Reason?.Trim() ?? string.Empty;
        var attachment = string.IsNullOrWhiteSpace(request.AttachmentPath) ? null : request.AttachmentPath.Trim();
        var numberOfDays = CalculateRequestedDays(request.StartDate, request.EndDate);
        return new LeaveRequest
        {
            Id = Guid.NewGuid(),
            EmployeeId = request.EmployeeId,
            LeaveTypeId = leaveType.Id,
            LeaveType = leaveType.Name,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            NumberOfDays = numberOfDays,
            Reason = reason,
            Status = LeaveRequestStatus.PendingApproval,
            AttachmentPath = attachment,
            SubmittedAtUtc = DateTime.UtcNow
        };
    }

    public static LeaveRequest ApplyUpdates(this UpdateLeaveRequest request, LeaveRequest existing, LeaveType leaveType)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(existing);
        ArgumentNullException.ThrowIfNull(leaveType);

        var reason = request.Reason?.Trim() ?? string.Empty;
        var attachment = string.IsNullOrWhiteSpace(request.AttachmentPath) ? null : request.AttachmentPath.Trim();
        var numberOfDays = CalculateRequestedDays(request.StartDate, request.EndDate);

        return new LeaveRequest
        {
            Id = existing.Id,
            EmployeeId = existing.EmployeeId,
            LeaveTypeId = leaveType.Id,
            LeaveType = leaveType.Name,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            NumberOfDays = numberOfDays,
            Reason = reason,
            Status = string.IsNullOrWhiteSpace(request.Status) ? existing.Status : request.Status.Trim(),
            ApproverId = request.ApproverId,
            AttachmentPath = attachment,
            SubmittedAtUtc = existing.SubmittedAtUtc,
            ApprovedAtUtc = request.ApprovedAtUtc,
            RejectedAtUtc = request.RejectedAtUtc,
            CancelledAtUtc = request.CancelledAtUtc
        };
    }

    private static decimal CalculateRequestedDays(DateOnly start, DateOnly end)
    {
        if (end < start)
            throw new ArgumentException("End date must be on or after the start date.");

        return end.DayNumber - start.DayNumber + 1;
    }
}
