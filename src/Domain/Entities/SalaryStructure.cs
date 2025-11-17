namespace HR.Domain.Entities;

/// <summary>
///     Salary structure assigned to an employee or job role.
/// </summary>
public sealed class SalaryStructure
{
    public static SalaryStructure Empty { get; } = new()
    {
        BasicSalary = 0,
        PaySchedule = "Monthly",
        Earnings = Array.Empty<SalaryComponent>(),
        Deductions = Array.Empty<SalaryComponent>()
    };

    public decimal BasicSalary { get; init; }

    public string PaySchedule { get; init; } = "Monthly";

    public IReadOnlyCollection<SalaryComponent> Earnings { get; init; } = Array.Empty<SalaryComponent>();

    public IReadOnlyCollection<SalaryComponent> Deductions { get; init; } = Array.Empty<SalaryComponent>();
}
