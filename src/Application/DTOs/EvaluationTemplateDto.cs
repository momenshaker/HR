namespace HR.Application.DTOs;

public sealed record EvaluationTemplateDto(
    Guid Id,
    string Name,
    string Description,
    string TargetRole,
    Guid RatingScaleId,
    bool IsDefault,
    bool IsActive,
    IReadOnlyCollection<TemplateSectionDefinitionDto> Sections
);

public sealed record TemplateSectionDefinitionDto(
    Guid Id,
    Guid TemplateId,
    string Name,
    decimal Weight,
    IReadOnlyCollection<TemplateItemDefinitionDto> Items
);

public sealed record TemplateItemDefinitionDto(
    Guid Id,
    Guid SectionDefinitionId,
    string Name,
    string Description,
    decimal DefaultWeight
);
