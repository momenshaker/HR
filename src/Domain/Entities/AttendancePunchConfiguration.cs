using System;

namespace HR.Domain.Entities;

public sealed class AttendancePunchConfiguration
{
    public Guid Id { get; init; }

    public string PunchType { get; init; } = string.Empty;

    public string DisplayName { get; init; } = string.Empty;

    public string Description { get; init; } = string.Empty;

    public int SortOrder { get; init; }

    public bool IsActive { get; init; }
}
