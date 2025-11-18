namespace HR.Domain.Entities;

public sealed record EvaluationTemplate
{
    public Guid Id { get; init; }

    public string Name { get; init; } = string.Empty;

    public string Description { get; init; } = string.Empty;

    public string TargetRole { get; init; } = string.Empty;

    public Guid RatingScaleId { get; init; }

    public bool IsDefault { get; init; }

    public bool IsActive { get; init; }

    public IReadOnlyCollection<TemplateSectionDefinition> Sections { get; init; } = Array.Empty<TemplateSectionDefinition>();
}

public sealed record TemplateSectionDefinition
{
    public Guid Id { get; init; }

    public Guid TemplateId { get; init; }

    public string Name { get; init; } = string.Empty;

    public decimal Weight { get; init; }

    public IReadOnlyCollection<TemplateItemDefinition> Items { get; init; } = Array.Empty<TemplateItemDefinition>();
}

public sealed record TemplateItemDefinition
{
    public Guid Id { get; init; }

    public Guid SectionDefinitionId { get; init; }

    public string Name { get; init; } = string.Empty;

    public string Description { get; init; } = string.Empty;

    public decimal DefaultWeight { get; init; }
}
