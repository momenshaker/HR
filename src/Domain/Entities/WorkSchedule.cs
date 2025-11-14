namespace HR.Domain.Entities;

/// <summary>
///     Represents a set of working rules that can be assigned to one or more employees.
/// </summary>
public sealed class WorkSchedule
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public Guid? OrganizationId { get; set; }

    public Guid? DepartmentId { get; set; }

    public bool IsDefaultForOrganization { get; set; }

    public string TimeZoneId { get; set; } = string.Empty;

    public ICollection<ShiftTemplate> ShiftTemplates { get; } = new List<ShiftTemplate>();
}
