using HR.Application.Abstractions.Repositories;
using HR.Application.Abstractions.Services;
using HR.Application.DTOs;
using HR.Application.Mappings;
using HR.Domain.Entities;

namespace HR.Application.Services;

/// <inheritdoc />
public sealed class PerformanceManagementService : IPerformanceManagementService
{
    private readonly IPerformanceRepository _performanceRepository;

    public PerformanceManagementService(IPerformanceRepository performanceRepository)
    {
        _performanceRepository = performanceRepository;
    }

    public async Task<IReadOnlyCollection<RatingScaleDto>> GetRatingScalesAsync(CancellationToken cancellationToken = default)
    {
        var scales = await _performanceRepository.GetRatingScalesAsync(cancellationToken).ConfigureAwait(false);
        return scales.Select(scale => scale.ToDto()).ToArray();
    }

    public async Task<IReadOnlyCollection<EvaluationTemplateDto>> GetTemplatesAsync(CancellationToken cancellationToken = default)
    {
        var templates = await _performanceRepository.GetTemplatesAsync(cancellationToken).ConfigureAwait(false);
        return templates.Select(template => template.ToDto()).ToArray();
    }

    public async Task<EvaluationTemplateDto?> GetTemplateAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var template = await _performanceRepository.GetTemplateAsync(id, cancellationToken).ConfigureAwait(false);
        return template?.ToDto();
    }

    public async Task<EvaluationTemplateDto> CreateTemplateAsync(CreateEvaluationTemplateRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var template = request.ToEntity();
        var created = await _performanceRepository.AddTemplateAsync(template, cancellationToken).ConfigureAwait(false);
        return created.ToDto();
    }

    public async Task<IReadOnlyCollection<PerformanceCycleDto>> GetCyclesAsync(CancellationToken cancellationToken = default)
    {
        var cycles = await _performanceRepository.GetCyclesAsync(cancellationToken).ConfigureAwait(false);

        var cycleDtos = new List<PerformanceCycleDto>(cycles.Count);
        foreach (var cycle in cycles)
        {
            var evaluations = await _performanceRepository.GetEvaluationsByCycleAsync(cycle.Id, cancellationToken).ConfigureAwait(false);
            cycleDtos.Add(cycle.ToDto(evaluations.Count));
        }

        return cycleDtos;
    }

    public async Task<PerformanceCycleDto?> GetCycleAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var cycle = await _performanceRepository.GetCycleAsync(id, cancellationToken).ConfigureAwait(false);
        if (cycle is null)
        {
            return null;
        }

        var evaluations = await _performanceRepository.GetEvaluationsByCycleAsync(id, cancellationToken).ConfigureAwait(false);
        return cycle.ToDto(evaluations.Count);
    }

    public async Task<PerformanceCycleDto> CreateCycleAsync(CreatePerformanceCycleRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var template = await _performanceRepository.GetTemplateAsync(request.TemplateId, cancellationToken).ConfigureAwait(false);
        var scale = await _performanceRepository.GetRatingScaleAsync(request.RatingScaleId, cancellationToken).ConfigureAwait(false);
        if (template is null || scale is null)
        {
            throw new InvalidOperationException("Templates and rating scales must exist before creating a cycle.");
        }

        var cycle = request.ToEntity();
        var created = await _performanceRepository.AddCycleAsync(cycle, cancellationToken).ConfigureAwait(false);
        return created.ToDto(0);
    }

    public async Task<PerformanceCycleDto?> UpdateCycleAsync(Guid id, UpdatePerformanceCycleRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var existing = await _performanceRepository.GetCycleAsync(id, cancellationToken).ConfigureAwait(false);
        if (existing is null)
        {
            return null;
        }

        var updated = request.ToUpdatedEntity(existing);
        var persisted = await _performanceRepository.UpdateCycleAsync(updated, cancellationToken).ConfigureAwait(false);

        if (persisted is null)
        {
            return null;
        }

        var evaluations = await _performanceRepository.GetEvaluationsByCycleAsync(id, cancellationToken).ConfigureAwait(false);
        return persisted.ToDto(evaluations.Count);
    }

    public async Task<PerformanceCycleDto?> ActivateCycleAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var cycle = await _performanceRepository.GetCycleAsync(id, cancellationToken).ConfigureAwait(false);
        if (cycle is null)
        {
            return null;
        }

        var template = await _performanceRepository.GetTemplateAsync(cycle.TemplateId, cancellationToken).ConfigureAwait(false);
        var ratingScale = await _performanceRepository.GetRatingScaleAsync(cycle.RatingScaleId, cancellationToken).ConfigureAwait(false);
        if (template is null || ratingScale is null)
        {
            return null;
        }

        foreach (var assignment in cycle.IncludedEmployees)
        {
            var evaluation = CreateEvaluationFromTemplate(cycle, template, assignment);
            await _performanceRepository.AddEvaluationAsync(evaluation, cancellationToken).ConfigureAwait(false);
        }

        var activated = cycle with { Status = PerformanceCycleStatus.Active };
        await _performanceRepository.UpdateCycleAsync(activated, cancellationToken).ConfigureAwait(false);

        var evaluations = await _performanceRepository.GetEvaluationsByCycleAsync(id, cancellationToken).ConfigureAwait(false);
        return activated.ToDto(evaluations.Count);
    }

    public async Task<PerformanceCycleDto?> CloseCycleAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var cycle = await _performanceRepository.GetCycleAsync(id, cancellationToken).ConfigureAwait(false);
        if (cycle is null)
        {
            return null;
        }

        var closed = cycle with { Status = PerformanceCycleStatus.Closed };
        await _performanceRepository.UpdateCycleAsync(closed, cancellationToken).ConfigureAwait(false);

        var evaluations = await _performanceRepository.GetEvaluationsByCycleAsync(id, cancellationToken).ConfigureAwait(false);
        return closed.ToDto(evaluations.Count);
    }

    public async Task<IReadOnlyCollection<EvaluationSummaryDto>> GetEvaluationsForCycleAsync(Guid cycleId, CancellationToken cancellationToken = default)
    {
        var cycle = await _performanceRepository.GetCycleAsync(cycleId, cancellationToken).ConfigureAwait(false);
        if (cycle is null)
        {
            return Array.Empty<EvaluationSummaryDto>();
        }

        var template = await _performanceRepository.GetTemplateAsync(cycle.TemplateId, cancellationToken).ConfigureAwait(false);
        var evaluations = await _performanceRepository.GetEvaluationsByCycleAsync(cycleId, cancellationToken).ConfigureAwait(false);

        return evaluations
            .Select(evaluation => evaluation.ToSummaryDto(cycle.Name, template?.Name ?? string.Empty))
            .ToArray();
    }

    public async Task<EvaluationDto?> GetEvaluationAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var evaluation = await _performanceRepository.GetEvaluationAsync(id, cancellationToken).ConfigureAwait(false);
        return evaluation?.ToDto();
    }

    public async Task<EvaluationDto?> SubmitSelfEvaluationAsync(Guid id, SubmitEvaluationRequest request, CancellationToken cancellationToken = default)
    {
        var evaluation = await _performanceRepository.GetEvaluationAsync(id, cancellationToken).ConfigureAwait(false);
        if (evaluation is null)
        {
            return null;
        }

        var updated = evaluation.ApplySelfSubmission(request);
        var persisted = await _performanceRepository.UpdateEvaluationAsync(updated, cancellationToken).ConfigureAwait(false);
        return persisted?.ToDto();
    }

    public async Task<EvaluationDto?> SubmitManagerEvaluationAsync(Guid id, SubmitEvaluationRequest request, CancellationToken cancellationToken = default)
    {
        var evaluation = await _performanceRepository.GetEvaluationAsync(id, cancellationToken).ConfigureAwait(false);
        if (evaluation is null)
        {
            return null;
        }

        var updated = evaluation.ApplyManagerSubmission(request);
        var persisted = await _performanceRepository.UpdateEvaluationAsync(updated, cancellationToken).ConfigureAwait(false);
        return persisted?.ToDto();
    }

    private static Evaluation CreateEvaluationFromTemplate(PerformanceCycle cycle, EvaluationTemplate template, PerformanceCycleAssignment assignment)
    {
        var evaluationId = Guid.NewGuid();
        var sectionInstances = template.Sections.Select(section =>
        {
            var items = section.Items.Select(item => new EvaluationItem
            {
                Id = Guid.NewGuid(),
                EvaluationSectionId = Guid.Empty,
                TemplateItemDefinitionId = item.Id,
                Name = item.Name,
                Weight = item.DefaultWeight
            }).ToArray();

            var sectionId = Guid.NewGuid();
            return new EvaluationSection
            {
                Id = sectionId,
                EvaluationId = evaluationId,
                TemplateSectionDefinitionId = section.Id,
                Name = section.Name,
                Weight = section.Weight,
                Items = items.Select(i => i with { EvaluationSectionId = sectionId }).ToArray()
            };
        }).ToArray();

        return new Evaluation
        {
            Id = evaluationId,
            EmployeeId = assignment.EmployeeId,
            ManagerId = assignment.ManagerId,
            CycleId = cycle.Id,
            TemplateId = template.Id,
            Status = EvaluationStatus.NotStarted,
            Sections = sectionInstances,
            Goals = Array.Empty<EvaluationGoal>(),
            Participants = new EvaluationParticipant[]
            {
                new() { Id = Guid.NewGuid(), EvaluationId = evaluationId, ParticipantEmployeeId = assignment.EmployeeId, Role = EvaluationParticipantRole.Self, Status = EvaluationStatus.NotStarted },
                new() { Id = Guid.NewGuid(), EvaluationId = evaluationId, ParticipantEmployeeId = assignment.ManagerId ?? Guid.Empty, Role = EvaluationParticipantRole.Manager, Status = EvaluationStatus.NotStarted }
            },
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        }.WithCalculatedScores();
    }
}
