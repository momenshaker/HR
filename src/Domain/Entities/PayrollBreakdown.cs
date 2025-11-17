using System.Text.Json;

namespace HR.Domain.Entities;

/// <summary>
///     Structured earnings and deductions for a payroll line.
/// </summary>
public sealed class PayrollBreakdown
{
    public static PayrollBreakdown Empty { get; } = new()
    {
        Earnings = Array.Empty<PayrollComponentAmount>(),
        Deductions = Array.Empty<PayrollComponentAmount>()
    };

    public required IReadOnlyCollection<PayrollComponentAmount> Earnings { get; init; }

    public required IReadOnlyCollection<PayrollComponentAmount> Deductions { get; init; }

    public static PayrollBreakdown FromJson(string? json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            return Empty;
        }

        return JsonSerializer.Deserialize<PayrollBreakdown>(json, SerializerOptions) ?? Empty;
    }

    public string ToJson()
    {
        return JsonSerializer.Serialize(this, SerializerOptions);
    }

    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };
}

/// <summary>
///     Monetary amount calculated for a specific pay component.
/// </summary>
public sealed class PayrollComponentAmount
{
    public string ComponentId { get; init; } = string.Empty;

    public string Name { get; init; } = string.Empty;

    public PayrollComponentType Type { get; init; } = PayrollComponentType.Earning;

    public PayrollCalculationType CalculationType { get; init; } = PayrollCalculationType.FixedAmount;

    public decimal Amount { get; init; }

    public bool IsTaxable { get; init; }

    public bool IsRecurring { get; init; }

    public string? Formula { get; init; }
}
