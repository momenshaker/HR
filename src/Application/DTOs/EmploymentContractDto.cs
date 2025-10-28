namespace HR.Application.DTOs;

/// <summary>
///     Read model representing an employment contract linked to an employee.
/// </summary>
public sealed record EmploymentContractDto(
    Guid Id,
    string ContractType,
    string ContractNumber,
    string Status,
    DateOnly EffectiveFrom,
    DateOnly? EffectiveTo,
    decimal? FtePercentage,
    string WorkLocation,
    string CompensationCurrency,
    decimal? AnnualCompensation,
    string Notes);
