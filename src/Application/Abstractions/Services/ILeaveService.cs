using HR.Application.DTOs;

namespace HR.Application.Abstractions.Services;

public interface ILeaveService
{
    Task<IReadOnlyCollection<LeaveTypeDto>> GetLeaveTypesAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<LeaveBalanceDto>> GetBalancesAsync(Guid employeeId, int year, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<LeaveBalanceDto>> SetBalancesAsync(SetLeaveBalancesRequest request, CancellationToken cancellationToken = default);

    Task<LeavePreviewDto> PreviewAsync(Guid employeeId, Guid leaveTypeId, DateOnly startDate, DateOnly endDate, CancellationToken cancellationToken = default);

    Task<LeaveRequestDto> SubmitAsync(SubmitLeaveRequest request, CancellationToken cancellationToken = default);

    Task<LeaveRequestDto> ApproveAsync(Guid requestId, Guid managerId, CancellationToken cancellationToken = default);

    Task<LeaveRequestDto> RejectAsync(Guid requestId, Guid managerId, string reason, CancellationToken cancellationToken = default);

    Task<LeaveRequestDto> CancelAsync(Guid requestId, Guid employeeId, CancellationToken cancellationToken = default);

    Task<PagedLeaveRequestsDto> GetRequestsAsync(Guid? employeeId, Guid? managerId, string? status, int page, int pageSize, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<LeaveApprovalStepDto>> CreateApprovalWorkflowAsync(CreateLeaveApprovalWorkflowRequest request, CancellationToken cancellationToken = default);
}
