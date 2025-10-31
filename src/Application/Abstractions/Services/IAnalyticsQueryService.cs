using HR.Application.DTOs;

namespace HR.Application.Abstractions.Services;

public interface IAnalyticsQueryService
{
    Task<IReadOnlyCollection<HeadcountItemDto>> GetHeadcountAsync(Guid organizationId, Guid? departmentId, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<UtilizationPeriodDto>> GetUtilizationAsync(Guid organizationId, DateOnly start, DateOnly end, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<LeaveUsageItemDto>> GetLeaveUsageAsync(Guid organizationId, int year, CancellationToken cancellationToken = default);

    Task<PayrollTotalsResponseDto> GetPayrollTotalsAsync(Guid organizationId, DateOnly start, DateOnly end, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<StageCountDto>> GetRecruitmentFunnelAsync(Guid vacancyId, CancellationToken cancellationToken = default);

    Task<TrainingComplianceDto> GetTrainingComplianceAsync(Guid organizationId, CancellationToken cancellationToken = default);
}
