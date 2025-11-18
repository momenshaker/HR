using System.Collections.Concurrent;
using HR.Application.Abstractions.Repositories;
using HR.Domain.Entities;

namespace HR.Infrastructure.Persistence.Repositories;

public sealed class InMemoryPerformanceRepository : IPerformanceRepository
{
    private readonly ConcurrentDictionary<Guid, PerformanceCycle> _cycles = new();
    private readonly ConcurrentDictionary<Guid, Evaluation> _evaluations = new();
    private readonly ConcurrentDictionary<Guid, RatingScale> _ratingScales = new();
    private readonly ConcurrentDictionary<Guid, EvaluationTemplate> _templates = new();

    public InMemoryPerformanceRepository()
    {
        var defaultScale = new RatingScale
        {
            Id = Guid.NewGuid(),
            Name = "Five Point",
            MinScore = 1,
            MaxScore = 5,
            AllowHalfPoints = true,
            Levels = new RatingScaleLevel[]
            {
                new() { Id = Guid.NewGuid(), RatingScaleId = Guid.Empty, Score = 1, Label = "Needs Improvement", Description = "Performance is below expectations" },
                new() { Id = Guid.NewGuid(), RatingScaleId = Guid.Empty, Score = 2, Label = "Developing", Description = "Inconsistently meets expectations" },
                new() { Id = Guid.NewGuid(), RatingScaleId = Guid.Empty, Score = 3, Label = "Meets Expectations", Description = "Delivering on commitments" },
                new() { Id = Guid.NewGuid(), RatingScaleId = Guid.Empty, Score = 4, Label = "Exceeds", Description = "Frequently exceeds expectations" },
                new() { Id = Guid.NewGuid(), RatingScaleId = Guid.Empty, Score = 5, Label = "Outstanding", Description = "Consistently exceptional performance" }
            }
        };

        defaultScale = defaultScale with
        {
            Levels = defaultScale.Levels.Select(level => level with { RatingScaleId = defaultScale.Id }).ToArray()
        };

        _ratingScales.TryAdd(defaultScale.Id, defaultScale);

        var defaultTemplate = new EvaluationTemplate
        {
            Id = Guid.NewGuid(),
            Name = "General Staff Template",
            Description = "Core competencies with goal alignment",
            TargetRole = "Staff",
            RatingScaleId = defaultScale.Id,
            IsDefault = true,
            IsActive = true,
            Sections = new TemplateSectionDefinition[]
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    TemplateId = Guid.Empty,
                    Name = "Core Competencies",
                    Weight = 60,
                    Items = new TemplateItemDefinition[]
                    {
                        new() { Id = Guid.NewGuid(), SectionDefinitionId = Guid.Empty, Name = "Collaboration", Description = "Works well across teams", DefaultWeight = 50 },
                        new() { Id = Guid.NewGuid(), SectionDefinitionId = Guid.Empty, Name = "Execution", Description = "Delivers on commitments", DefaultWeight = 50 }
                    }
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    TemplateId = Guid.Empty,
                    Name = "Growth",
                    Weight = 40,
                    Items = new TemplateItemDefinition[]
                    {
                        new() { Id = Guid.NewGuid(), SectionDefinitionId = Guid.Empty, Name = "Learning", Description = "Learns new skills", DefaultWeight = 50 },
                        new() { Id = Guid.NewGuid(), SectionDefinitionId = Guid.Empty, Name = "Impact", Description = "Positive impact on OKRs", DefaultWeight = 50 }
                    }
                }
            }
        };

        defaultTemplate = NormalizeTemplate(defaultTemplate);
        _templates.TryAdd(defaultTemplate.Id, defaultTemplate);
    }

    public Task<IReadOnlyCollection<RatingScale>> GetRatingScalesAsync(CancellationToken cancellationToken = default)
    {
        IReadOnlyCollection<RatingScale> snapshot = _ratingScales.Values.ToArray();
        return Task.FromResult(snapshot);
    }

    public Task<RatingScale?> GetRatingScaleAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _ratingScales.TryGetValue(id, out var scale);
        return Task.FromResult(scale);
    }

    public Task<IReadOnlyCollection<EvaluationTemplate>> GetTemplatesAsync(CancellationToken cancellationToken = default)
    {
        IReadOnlyCollection<EvaluationTemplate> snapshot = _templates.Values.ToArray();
        return Task.FromResult(snapshot);
    }

    public Task<EvaluationTemplate?> GetTemplateAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _templates.TryGetValue(id, out var template);
        return Task.FromResult(template);
    }

    public Task<EvaluationTemplate> AddTemplateAsync(EvaluationTemplate template, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(template);

        var normalized = NormalizeTemplate(template);
        _templates[normalized.Id] = normalized;

        return Task.FromResult(normalized);
    }

    public Task<IReadOnlyCollection<PerformanceCycle>> GetCyclesAsync(CancellationToken cancellationToken = default)
    {
        IReadOnlyCollection<PerformanceCycle> snapshot = _cycles.Values.ToArray();
        return Task.FromResult(snapshot);
    }

    public Task<PerformanceCycle?> GetCycleAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _cycles.TryGetValue(id, out var cycle);
        return Task.FromResult(cycle);
    }

    public Task<PerformanceCycle> AddCycleAsync(PerformanceCycle cycle, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(cycle);

        _cycles[cycle.Id] = cycle;
        return Task.FromResult(cycle);
    }

    public Task<PerformanceCycle?> UpdateCycleAsync(PerformanceCycle cycle, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(cycle);

        if (!_cycles.ContainsKey(cycle.Id))
        {
            return Task.FromResult<PerformanceCycle?>(null);
        }

        _cycles[cycle.Id] = cycle;
        return Task.FromResult<PerformanceCycle?>(cycle);
    }

    public Task<IReadOnlyCollection<Evaluation>> GetEvaluationsByCycleAsync(Guid cycleId, CancellationToken cancellationToken = default)
    {
        IReadOnlyCollection<Evaluation> snapshot = _evaluations.Values.Where(evaluation => evaluation.CycleId == cycleId).ToArray();
        return Task.FromResult(snapshot);
    }

    public Task<Evaluation?> GetEvaluationAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _evaluations.TryGetValue(id, out var evaluation);
        return Task.FromResult(evaluation);
    }

    public Task<Evaluation> AddEvaluationAsync(Evaluation evaluation, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(evaluation);

        _evaluations[evaluation.Id] = evaluation;
        return Task.FromResult(evaluation);
    }

    public Task<Evaluation?> UpdateEvaluationAsync(Evaluation evaluation, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(evaluation);

        if (!_evaluations.ContainsKey(evaluation.Id))
        {
            return Task.FromResult<Evaluation?>(null);
        }

        _evaluations[evaluation.Id] = evaluation;
        return Task.FromResult<Evaluation?>(evaluation);
    }

    private static EvaluationTemplate NormalizeTemplate(EvaluationTemplate template)
    {
        var normalizedSections = template.Sections.Select(section =>
        {
            var normalizedSectionId = section.Id == Guid.Empty ? Guid.NewGuid() : section.Id;
            var normalizedItems = section.Items.Select(item =>
            {
                var itemId = item.Id == Guid.Empty ? Guid.NewGuid() : item.Id;
                return item with
                {
                    Id = itemId,
                    SectionDefinitionId = normalizedSectionId
                };
            }).ToArray();

            return section with
            {
                Id = normalizedSectionId,
                TemplateId = template.Id,
                Items = normalizedItems
            };
        }).ToArray();

        return template with { Sections = normalizedSections };
    }
}
