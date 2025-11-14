namespace HR.Domain.Entities;

/// <summary>
///     Represents a corporate or public holiday that influences attendance.
/// </summary>
public sealed class Holiday
{
    public Guid Id { get; set; }

    public Guid OrganizationId { get; set; }

    public DateOnly Date { get; set; }

    public string Name { get; set; } = string.Empty;

    public bool IsPaid { get; set; }

    public string CountryCode { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;
}
