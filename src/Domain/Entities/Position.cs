using System;

namespace HR.Domain.Entities;

/// <summary>
///     Represents a role or job position within an organisation unit.
/// </summary>
public sealed class Position
{
    public Guid Id { get; init; }

    public string Title { get; init; } = string.Empty;

    public string JobCode { get; init; } = string.Empty;

    public Guid OrganizationUnitId { get; init; }

    public Guid? ReportsToPositionId { get; init; }

    public string Grade { get; init; } = string.Empty;

    public string EmploymentType { get; init; } = string.Empty;

    public DateOnly? EffectiveFrom { get; init; }

    public DateOnly? EffectiveTo { get; init; }

    public bool IsCriticalRole { get; init; }

    public bool IsVacant { get; init; }
}
