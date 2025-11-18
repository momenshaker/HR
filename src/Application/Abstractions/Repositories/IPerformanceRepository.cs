using HR.Domain.Entities;

namespace HR.Application.Abstractions.Repositories;

public interface IPerformanceRepository
{
    Task<IReadOnlyCollection<RatingScale>> GetRatingScalesAsync(CancellationToken cancellationToken = default);

    Task<RatingScale?> GetRatingScaleAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<EvaluationTemplate>> GetTemplatesAsync(CancellationToken cancellationToken = default);

    Task<EvaluationTemplate?> GetTemplateAsync(Guid id, CancellationToken cancellationToken = default);

    Task<EvaluationTemplate> AddTemplateAsync(EvaluationTemplate template, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<PerformanceCycle>> GetCyclesAsync(CancellationToken cancellationToken = default);

    Task<PerformanceCycle?> GetCycleAsync(Guid id, CancellationToken cancellationToken = default);

    Task<PerformanceCycle> AddCycleAsync(PerformanceCycle cycle, CancellationToken cancellationToken = default);

    Task<PerformanceCycle?> UpdateCycleAsync(PerformanceCycle cycle, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<Evaluation>> GetEvaluationsByCycleAsync(Guid cycleId, CancellationToken cancellationToken = default);

    Task<Evaluation?> GetEvaluationAsync(Guid id, CancellationToken cancellationToken = default);

    Task<Evaluation> AddEvaluationAsync(Evaluation evaluation, CancellationToken cancellationToken = default);

    Task<Evaluation?> UpdateEvaluationAsync(Evaluation evaluation, CancellationToken cancellationToken = default);
}
