namespace HR.Domain.Entities;

/// <summary>
///     Represents a contractual agreement governing an employee's engagement with the organisation.
/// </summary>
public sealed class EmploymentContract
{
    /// <summary>
    ///     Gets the unique identifier for the contract instance.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    ///     Gets the descriptive contract type (e.g. Permanent, Fixed-Term, Contractor).
    /// </summary>
    public string ContractType { get; init; } = string.Empty;

    /// <summary>
    ///     Gets an optional human-friendly contract or document reference number.
    /// </summary>
    public string ContractNumber { get; init; } = string.Empty;

    /// <summary>
    ///     Gets the lifecycle status for the contract (e.g. Active, Pending, Expired).
    /// </summary>
    public string Status { get; init; } = string.Empty;

    /// <summary>
    ///     Gets the effective start date for the contractual engagement.
    /// </summary>
    public DateOnly EffectiveFrom { get; init; }

    /// <summary>
    ///     Gets the optional effective end date when the contract expires or is terminated.
    /// </summary>
    public DateOnly? EffectiveTo { get; init; }

    /// <summary>
    ///     Gets the proportion of a full-time schedule allocated to this contract (0.0 – 1.0).
    /// </summary>
    public decimal? FtePercentage { get; init; }

    /// <summary>
    ///     Gets the primary work location or country associated with the contract.
    /// </summary>
    public string WorkLocation { get; init; } = string.Empty;

    /// <summary>
    ///     Gets the compensation currency code (ISO 4217) tied to the remuneration values.
    /// </summary>
    public string CompensationCurrency { get; init; } = string.Empty;

    /// <summary>
    ///     Gets the annualised base compensation for the contract when available.
    /// </summary>
    public decimal? AnnualCompensation { get; init; }

    /// <summary>
    ///     Gets optional free-form notes or narrative supporting the contract record.
    /// </summary>
    public string Notes { get; init; } = string.Empty;
}
