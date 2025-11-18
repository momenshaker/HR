using HR.Application.Validation;

namespace HR.Application.DTOs;

public sealed class CreateEvaluationTemplateRequest : IValidatableRequest
{
    public string Name { get; init; } = string.Empty;

    public string Description { get; init; } = string.Empty;

    public string TargetRole { get; init; } = string.Empty;

    public Guid RatingScaleId { get; init; }

    public bool IsDefault { get; init; }

    public bool IsActive { get; init; }

    public IReadOnlyCollection<TemplateSectionDefinitionRequest> Sections { get; init; } = Array.Empty<TemplateSectionDefinitionRequest>();
}

public sealed class TemplateSectionDefinitionRequest
{
    public string Name { get; init; } = string.Empty;

    public decimal Weight { get; init; }

    public IReadOnlyCollection<TemplateItemDefinitionRequest> Items { get; init; } = Array.Empty<TemplateItemDefinitionRequest>();
}

public sealed class TemplateItemDefinitionRequest
{
    public string Name { get; init; } = string.Empty;

    public string Description { get; init; } = string.Empty;

    public decimal DefaultWeight { get; init; }
}
