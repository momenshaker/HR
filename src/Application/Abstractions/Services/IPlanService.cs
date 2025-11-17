using HR.Application.DTOs;

namespace HR.Application.Abstractions.Services;

/// <summary>
///     Provides read access to available subscription plans.
/// </summary>
public interface IPlanService
{
    Task<IReadOnlyCollection<PlanDto>> GetPlansAsync(CancellationToken cancellationToken = default);
    Task<PlanDto> CreatePlanAsync(CreatePlanRequest request, CancellationToken cancellationToken = default);
    Task<PlanDto?> UpdatePlanAsync(Guid id, UpdatePlanRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeletePlanAsync(Guid id, CancellationToken cancellationToken = default);
}
