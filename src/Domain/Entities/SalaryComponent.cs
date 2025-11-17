namespace HR.Domain.Entities;

/// <summary>
///     Defines an earning or deduction component within an employee salary structure.
/// </summary>
public sealed class SalaryComponent
{
    public Guid Id { get; init; } = Guid.NewGuid();

    /// <summary>
    ///     Human readable component name (e.g., Housing Allowance, Tax).
    /// </summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>
    ///     Component category: Earning or Deduction.
    /// </summary>
    public PayrollComponentType Type { get; init; } = PayrollComponentType.Earning;

    /// <summary>
    ///     Calculation strategy: fixed amount, percentage of base/gross, or formula-based.
    /// </summary>
    public PayrollCalculationType CalculationType { get; init; } = PayrollCalculationType.FixedAmount;

    /// <summary>
    ///     Numeric value used in calculation (amount or percentage multiplier).
    /// </summary>
    public decimal Value { get; init; }

    public bool IsTaxable { get; init; }

    public bool IsRecurring { get; init; } = true;

    /// <summary>
    ///     Optional formula token (e.g., Overtime, UnpaidLeave) evaluated by the payroll calculator.
    /// </summary>
    public string? Formula { get; init; }
}

public enum PayrollComponentType
{
    Earning,
    Deduction
}

public enum PayrollCalculationType
{
    FixedAmount,
    PercentageOfBasic,
    PercentageOfGross,
    Formula
}
