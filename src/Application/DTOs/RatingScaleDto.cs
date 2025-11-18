using HR.Domain.Entities;

namespace HR.Application.DTOs;

public sealed record RatingScaleDto(
    Guid Id,
    string Name,
    decimal MinScore,
    decimal MaxScore,
    bool AllowHalfPoints,
    IReadOnlyCollection<RatingScaleLevelDto> Levels
);

public sealed record RatingScaleLevelDto(Guid Id, Guid RatingScaleId, decimal Score, string Label, string Description);
