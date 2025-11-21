using System;
using System.Collections.Generic;
using System.Linq;

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
    IApprovalStepRepository approvalStepRepository,
    ILeaveTypeRepository leaveTypeRepository,
    ILeaveBalanceRepository leaveBalanceRepository,
    IWorkdayCalendar calendar) : ILeaveService
{
    private readonly ILeaveRequestRepository _leaveRequests = leaveRequestRepository;
    private readonly IApprovalStepRepository _approvalSteps = approvalStepRepository;
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
            .Where(r => r.EmployeeId == employeeId &&
                        string.Equals(r.Status, LeaveRequestStatus.PendingApproval, StringComparison.OrdinalIgnoreCase))
            .GroupBy(r => new { Year = r.StartDate.Year, r.LeaveTypeId })
            .ToDictionary(
                g => (g.Key.Year, g.Key.LeaveTypeId),
                g => g.Sum(GetRecordedDuration));

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

            var reserved = reservedByType.TryGetValue((b.Year, lt.Id), out var res) ? res : 0m;
            result.Add(b.ToDto(reserved));
        }

        return result;
    }

    public async Task<IReadOnlyCollection<LeaveBalanceDto>> SetBalancesAsync(SetLeaveBalancesRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        if (request.Balances is null || request.Balances.Count == 0)
        {
            throw new ArgumentException("Specify at least one balance to update.", nameof(request));
        }

        var leaveTypes = await _leaveTypes.GetAllAsync(cancellationToken).ConfigureAwait(false);
        var typeMap = leaveTypes.ToDictionary(t => t.Id);

        foreach (var entry in request.Balances)
        {
            if (entry.Remaining < 0)
            {
                throw new ArgumentException("Remaining balance cannot be negative.", nameof(entry));
            }

            if (!typeMap.TryGetValue(entry.LeaveTypeId, out var leaveType))
            {
                throw new InvalidOperationException("Leave type not found.");
            }

            await AdjustBalanceAsync(request.EmployeeId, request.Year, leaveType, entry.Remaining, cancellationToken).ConfigureAwait(false);
        }

        var updated = await GetBalancesAsync(request.EmployeeId, request.Year, cancellationToken).ConfigureAwait(false);
        var requestedTypeIds = request.Balances.Select(b => b.LeaveTypeId).ToHashSet();
        return updated.Where(b => requestedTypeIds.Contains(b.LeaveTypeId)).ToArray();
    }

    private async Task AdjustBalanceAsync(Guid employeeId, int year, LeaveType leaveType, decimal targetRemaining, CancellationToken cancellationToken)
    {
        var current = await _leaveBalances.GetAsync(employeeId, leaveType.Id, year, cancellationToken).ConfigureAwait(false);
        var baseBalance = current ?? new LeaveBalance
        {
            EmployeeId = employeeId,
            LeaveTypeId = leaveType.Id,
            Year = year,
            Opening = 0,
            Accrued = leaveType.AnnualAllowanceDays,
            Taken = 0,
            CarriedOver = 0
        };

        var totalCredit = baseBalance.Opening + baseBalance.Accrued + baseBalance.CarriedOver;
        var newOpening = baseBalance.Opening;
        decimal newTaken;

        if (targetRemaining <= totalCredit)
        {
            newTaken = totalCredit - targetRemaining;
        }
        else
        {
            var extra = targetRemaining - totalCredit;
            newOpening += extra;
            newTaken = 0;
        }

        if (newTaken < 0)
        {
            newTaken = 0;
        }

        var updated = new LeaveBalance
        {
            EmployeeId = baseBalance.EmployeeId,
            LeaveTypeId = baseBalance.LeaveTypeId,
            Year = baseBalance.Year,
            Opening = newOpening,
            Accrued = baseBalance.Accrued,
            Taken = newTaken,
            CarriedOver = baseBalance.CarriedOver,
            RowVersion = baseBalance.RowVersion
        };

        await _leaveBalances.UpsertAsync(updated, cancellationToken).ConfigureAwait(false);
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

        var reserved = await GetReservedAsync(employeeId, type.Id, year, cancellationToken).ConfigureAwait(false);

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

        if (leaveType.MaxConsecutiveDays.HasValue)
        {
            var inclusiveDays = (request.EndDate.DayNumber - request.StartDate.DayNumber) + 1;
            if (inclusiveDays > leaveType.MaxConsecutiveDays.Value)
            {
                throw new InvalidOperationException("Requested range exceeds the maximum consecutive days permitted for this leave type.");
            }
        }

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

        if (leaveType.RequiresAttachment && string.IsNullOrWhiteSpace(request.AttachmentPath))
        {
            throw new InvalidOperationException("This leave type requires an attachment.");
        }

        var attachmentPath = string.IsNullOrWhiteSpace(request.AttachmentPath) ? null : request.AttachmentPath.Trim();
        var entity = new LeaveRequest
        {
            Id = Guid.NewGuid(),
            EmployeeId = request.EmployeeId,
            LeaveTypeId = leaveType.Id,
            LeaveType = leaveType.Name,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            NumberOfDays = preview.DurationDays,
            Reason = request.Reason?.Trim() ?? string.Empty,
            Status = leaveType.RequiresApproval ? LeaveRequestStatus.PendingApproval : LeaveRequestStatus.Approved,
            ApproverId = null,
            AttachmentPath = attachmentPath,
            SubmittedAtUtc = DateTime.UtcNow,
            ApprovedAtUtc = leaveType.RequiresApproval ? null : DateTime.UtcNow
        };

        var created = await _leaveRequests.AddAsync(entity, cancellationToken).ConfigureAwait(false);

        // If auto-approved (no approval required), immediately update Taken balance.
        if (!leaveType.RequiresApproval)
        {
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

            var newTaken = balance.Taken + preview.DurationDays;
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

    public async Task<IReadOnlyCollection<LeaveApprovalStepDto>> CreateApprovalWorkflowAsync(CreateLeaveApprovalWorkflowRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (request.Steps is null || request.Steps.Count == 0)
        {
            throw new ArgumentException("Provide at least one approver to build a workflow.", nameof(request.Steps));
        }

        var leaveRequest = await _leaveRequests.GetByIdAsync(request.LeaveRequestId, cancellationToken).ConfigureAwait(false)
                           ?? throw new InvalidOperationException("Leave request not found.");

        if (leaveRequest.Status.Equals(LeaveRequestStatus.Cancelled, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Cannot create a workflow for a cancelled request.");
        }

        var orderedSteps = request.Steps
            .Select((s, index) => new
            {
                s.ApproverId,
                Order = s.StepOrder ?? index + 1
            })
            .OrderBy(s => s.Order)
            .ToList();

        if (orderedSteps.Select(s => s.Order).Distinct().Count() != orderedSteps.Count)
        {
            throw new InvalidOperationException("Duplicate step orders are not allowed in a workflow.");
        }

        var workflow = orderedSteps
            .Select((s, index) => new ApprovalStep
            {
                Id = Guid.NewGuid(),
                LeaveRequestId = request.LeaveRequestId,
                StepOrder = s.Order,
                ApproverId = s.ApproverId,
                Status = index == 0 ? ApprovalStepStatus.Pending : ApprovalStepStatus.NotStarted,
                Comment = string.Empty
            })
            .ToArray();

        var persisted = await _approvalSteps.ReplaceWorkflowAsync(request.LeaveRequestId, workflow, cancellationToken)
            .ConfigureAwait(false);

        return persisted.Select(x => x.ToDto()).ToArray();
    }

    public async Task<LeaveRequestDto> ApproveAsync(Guid requestId, Guid managerId, CancellationToken cancellationToken = default)
    {
        var request = await _leaveRequests.GetByIdAsync(requestId, cancellationToken).ConfigureAwait(false)
                      ?? throw new InvalidOperationException("Leave request not found.");
        if (!string.Equals(request.Status, LeaveRequestStatus.PendingApproval, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Only pending requests can be approved.");
        }

        var workflowSteps = await _approvalSteps.GetByLeaveRequestIdAsync(requestId, cancellationToken).ConfigureAwait(false);
        if (workflowSteps.Count > 0)
        {
            var orderedSteps = workflowSteps.OrderBy(s => s.StepOrder).ToList();
            var pendingStep = orderedSteps.FirstOrDefault(s => s.Status.Equals(ApprovalStepStatus.Pending, StringComparison.OrdinalIgnoreCase));

            if (pendingStep is null)
            {
                throw new InvalidOperationException("No pending approval step is available for this request.");
            }

            if (pendingStep.ApproverId != managerId)
            {
                throw new InvalidOperationException("This leave request is not assigned to the specified approver.");
            }

            var approvedStep = new ApprovalStep
            {
                Id = pendingStep.Id,
                LeaveRequestId = pendingStep.LeaveRequestId,
                StepOrder = pendingStep.StepOrder,
                ApproverId = pendingStep.ApproverId,
                Status = ApprovalStepStatus.Approved,
                ActionAtUtc = DateTime.UtcNow,
                Comment = pendingStep.Comment
            };

            await _approvalSteps.UpdateAsync(approvedStep, cancellationToken).ConfigureAwait(false);

            var nextStep = orderedSteps
                .FirstOrDefault(s => s.StepOrder > pendingStep.StepOrder && s.Status.Equals(ApprovalStepStatus.NotStarted, StringComparison.OrdinalIgnoreCase));

            if (nextStep is not null)
            {
                var pendingNext = new ApprovalStep
                {
                    Id = nextStep.Id,
                    LeaveRequestId = nextStep.LeaveRequestId,
                    StepOrder = nextStep.StepOrder,
                    ApproverId = nextStep.ApproverId,
                    Status = ApprovalStepStatus.Pending,
                    Comment = nextStep.Comment
                };

                await _approvalSteps.UpdateAsync(pendingNext, cancellationToken).ConfigureAwait(false);

                return request.ToDto();
            }
        }

        // Confirm deduction
        var year = request.StartDate.Year;
        var leaveType = await _leaveTypes.GetByIdAsync(request.LeaveTypeId, cancellationToken).ConfigureAwait(false)
                       ?? throw new InvalidOperationException("Leave type not found.");

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

        var duration = GetRecordedDuration(request);
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
            LeaveTypeId = request.LeaveTypeId,
            LeaveType = request.LeaveType,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            Status = LeaveRequestStatus.Approved,
            ApproverId = managerId,
            Reason = request.Reason,
            AttachmentPath = request.AttachmentPath,
            SubmittedAtUtc = request.SubmittedAtUtc,
            ApprovedAtUtc = DateTime.UtcNow,
            RejectedAtUtc = null,
            CancelledAtUtc = null
        };

        var persisted = await _leaveRequests.UpdateAsync(updated, cancellationToken).ConfigureAwait(false)
                        ?? throw new InvalidOperationException("Failed to update leave request.");

        return persisted.ToDto();
    }

    public async Task<LeaveRequestDto> RejectAsync(Guid requestId, Guid managerId, string reason, CancellationToken cancellationToken = default)
    {
        var request = await _leaveRequests.GetByIdAsync(requestId, cancellationToken).ConfigureAwait(false)
                      ?? throw new InvalidOperationException("Leave request not found.");
        if (!string.Equals(request.Status, LeaveRequestStatus.PendingApproval, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Only pending requests can be rejected.");
        }

        var workflowSteps = await _approvalSteps.GetByLeaveRequestIdAsync(requestId, cancellationToken).ConfigureAwait(false);
        if (workflowSteps.Count > 0)
        {
            var orderedSteps = workflowSteps.OrderBy(s => s.StepOrder).ToList();
            var pendingStep = orderedSteps.FirstOrDefault(s => s.Status.Equals(ApprovalStepStatus.Pending, StringComparison.OrdinalIgnoreCase));

            if (pendingStep is null)
            {
                throw new InvalidOperationException("No pending approval step is available for this request.");
            }

            if (pendingStep.ApproverId != managerId)
            {
                throw new InvalidOperationException("This leave request is not assigned to the specified approver.");
            }

            var rejectedStep = new ApprovalStep
            {
                Id = pendingStep.Id,
                LeaveRequestId = pendingStep.LeaveRequestId,
                StepOrder = pendingStep.StepOrder,
                ApproverId = pendingStep.ApproverId,
                Status = ApprovalStepStatus.Rejected,
                ActionAtUtc = DateTime.UtcNow,
                Comment = string.IsNullOrWhiteSpace(reason) ? pendingStep.Comment : reason.Trim()
            };

            await _approvalSteps.UpdateAsync(rejectedStep, cancellationToken).ConfigureAwait(false);
        }

        var updated = new LeaveRequest
        {
            Id = request.Id,
            EmployeeId = request.EmployeeId,
            LeaveTypeId = request.LeaveTypeId,
            LeaveType = request.LeaveType,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            Status = LeaveRequestStatus.Rejected,
            ApproverId = managerId,
            Reason = string.IsNullOrWhiteSpace(reason) ? request.Reason : reason.Trim(),
            AttachmentPath = request.AttachmentPath,
            SubmittedAtUtc = request.SubmittedAtUtc,
            ApprovedAtUtc = null,
            RejectedAtUtc = DateTime.UtcNow,
            CancelledAtUtc = null
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

        if (!string.Equals(request.Status, LeaveRequestStatus.PendingApproval, StringComparison.OrdinalIgnoreCase) &&
            !string.Equals(request.Status, LeaveRequestStatus.Approved, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Only pending or approved requests can be cancelled.");
        }

        // Only future requests can be cancelled
        if (request.StartDate <= DateOnly.FromDateTime(DateTime.UtcNow.Date))
        {
            throw new InvalidOperationException("Only future leave can be cancelled.");
        }

        // If previously approved, revert taken days
        if (string.Equals(request.Status, LeaveRequestStatus.Approved, StringComparison.OrdinalIgnoreCase))
        {
            var leaveType = await _leaveTypes.GetByIdAsync(request.LeaveTypeId, cancellationToken).ConfigureAwait(false)
                           ?? throw new InvalidOperationException("Leave type not found.");

            var year = request.StartDate.Year;
            var duration = GetRecordedDuration(request);
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
            LeaveTypeId = request.LeaveTypeId,
            LeaveType = request.LeaveType,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            Status = LeaveRequestStatus.Cancelled,
            ApproverId = request.ApproverId,
            Reason = request.Reason,
            AttachmentPath = request.AttachmentPath,
            SubmittedAtUtc = request.SubmittedAtUtc,
            ApprovedAtUtc = request.ApprovedAtUtc,
            RejectedAtUtc = request.RejectedAtUtc,
            CancelledAtUtc = DateTime.UtcNow
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
        var managerAssignments = managerId is null
            ? Array.Empty<ApprovalStep>()
            : await _approvalSteps.GetByApproverAsync(managerId.Value, cancellationToken).ConfigureAwait(false);

        var filtered = requests.AsEnumerable();
        if (employeeId is not null)
            filtered = filtered.Where(r => r.EmployeeId == employeeId);
        if (managerId is not null)
        {
            var managerPendingRequestIds = managerAssignments
                .Where(s => s.Status.Equals(ApprovalStepStatus.Pending, StringComparison.OrdinalIgnoreCase))
                .Select(s => s.LeaveRequestId)
                .ToHashSet();

            filtered = filtered.Where(r => r.ApproverId == managerId || managerPendingRequestIds.Contains(r.Id));
        }
        if (!string.IsNullOrWhiteSpace(status))
            filtered = filtered.Where(r => string.Equals(r.Status, status, StringComparison.OrdinalIgnoreCase));

        var total = filtered.Count();
        var items = filtered
            .OrderByDescending(r => r.SubmittedAtUtc)
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
            .Where(r => r.Status.Equals(LeaveRequestStatus.PendingApproval, StringComparison.OrdinalIgnoreCase) ||
                        r.Status.Equals(LeaveRequestStatus.Approved, StringComparison.OrdinalIgnoreCase))
            .Where(r => DateRangesOverlap(start, end, r.StartDate, r.EndDate))
            .FirstOrDefault();

        return overlapping is null ? null : "Overlapping leave request exists.";
    }

    private async Task<decimal> GetReservedAsync(Guid employeeId, Guid leaveTypeId, int year, CancellationToken ct)
    {
        var requests = await _leaveRequests.GetAllAsync(ct).ConfigureAwait(false);
        return requests
            .Where(r => r.EmployeeId == employeeId &&
                        r.Status.Equals(LeaveRequestStatus.PendingApproval, StringComparison.OrdinalIgnoreCase))
            .Where(r => r.StartDate.Year == year)
            .Where(r => r.LeaveTypeId == leaveTypeId)
            .Sum(GetRecordedDuration);
    }

    private decimal GetRecordedDuration(LeaveRequest request)
    {
        return request.NumberOfDays > 0
            ? request.NumberOfDays
            : CalculateDuration(request.StartDate, request.EndDate);
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

