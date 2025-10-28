using System.ComponentModel.DataAnnotations;

namespace HR.Application.DTOs;

/// <summary>
///     Request payload describing an employment contract entry for create/update operations.
/// </summary>
public sealed class EmploymentContractRequest
{
    public Guid? Id { get; init; }

    [Required]
    [MaxLength(100)]
    public string ContractType { get; init; } = string.Empty;

    [MaxLength(100)]
    public string ContractNumber { get; init; } = string.Empty;

    [MaxLength(50)]
    public string Status { get; init; } = string.Empty;

    [Required]
    public DateOnly EffectiveFrom { get; init; }

    public DateOnly? EffectiveTo { get; init; }

    [Range(typeof(decimal), "0", "1")]
    public decimal? FtePercentage { get; init; }

    [MaxLength(100)]
    public string WorkLocation { get; init; } = string.Empty;

    [MaxLength(3)]
    public string CompensationCurrency { get; init; } = string.Empty;

    [Range(typeof(decimal), "0", "79228162514264337593543950335")]
    public decimal? AnnualCompensation { get; init; }

    [MaxLength(500)]
    public string Notes { get; init; } = string.Empty;
}
