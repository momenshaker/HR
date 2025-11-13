namespace HR.Domain.Entities;

/// <summary>
///     Represents a configurable lookup value that can be managed by administrators.
/// </summary>
public sealed class LookupValue
{
    public Guid Id { get; set; }

    public string Category { get; set; } = string.Empty;

    public string Code { get; set; } = string.Empty;

    public string DisplayName { get; set; } = string.Empty;

    public string? Description { get; set; }

    public int SortOrder { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAtUtc { get; set; }

    public DateTime UpdatedAtUtc { get; set; }
}
