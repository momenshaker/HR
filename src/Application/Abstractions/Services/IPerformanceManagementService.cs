using HR.Application.DTOs;

namespace HR.Application.Abstractions.Services;

/// <summary>
///     Application service responsible for performance management operations.
/// </summary>
public interface IPerformanceManagementService
{
    Task<IReadOnlyCollection<RatingScaleDto>> GetRatingScalesAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<EvaluationTemplateDto>> GetTemplatesAsync(CancellationToken cancellationToken = default);

    Task<EvaluationTemplateDto?> GetTemplateAsync(Guid id, CancellationToken cancellationToken = default);

    Task<EvaluationTemplateDto> CreateTemplateAsync(CreateEvaluationTemplateRequest request, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<PerformanceCycleDto>> GetCyclesAsync(CancellationToken cancellationToken = default);

    Task<PerformanceCycleDto?> GetCycleAsync(Guid id, CancellationToken cancellationToken = default);

    Task<PerformanceCycleDto> CreateCycleAsync(CreatePerformanceCycleRequest request, CancellationToken cancellationToken = default);

    Task<PerformanceCycleDto?> UpdateCycleAsync(Guid id, UpdatePerformanceCycleRequest request, CancellationToken cancellationToken = default);

    Task<PerformanceCycleDto?> ActivateCycleAsync(Guid id, CancellationToken cancellationToken = default);

    Task<PerformanceCycleDto?> CloseCycleAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<EvaluationSummaryDto>> GetEvaluationsForCycleAsync(Guid cycleId, CancellationToken cancellationToken = default);

    Task<EvaluationDto?> GetEvaluationAsync(Guid id, CancellationToken cancellationToken = default);

    Task<EvaluationDto?> SubmitSelfEvaluationAsync(Guid id, SubmitEvaluationRequest request, CancellationToken cancellationToken = default);

    Task<EvaluationDto?> SubmitManagerEvaluationAsync(Guid id, SubmitEvaluationRequest request, CancellationToken cancellationToken = default);
}
