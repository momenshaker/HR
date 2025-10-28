namespace HR.Application.DTOs;

/// <summary>
///     Read model representing a key performance indicator.
/// </summary>
public sealed record PerformanceKpiDto(
    Guid Id,
    string Name,
    decimal TargetValue,
    decimal ActualValue,
    string UnitOfMeasure,
    string Status);
