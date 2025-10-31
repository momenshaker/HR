using System.ComponentModel.DataAnnotations;

namespace HR.Application.DTOs;

/// <summary>
///     Request payload that carries a collection of department identifiers.
/// </summary>
public sealed class EmployeeDepartmentIdentifiersRequest
{
    [Required]
    public IReadOnlyCollection<Guid> DepartmentIds { get; init; } = Array.Empty<Guid>();
}
