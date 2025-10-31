namespace HR.Application.DTOs;

/// <summary>
///     Incoming payload for moving a department within its organization hierarchy.
/// </summary>
public sealed class MoveDepartmentRequest
{
    /// <summary>
    ///     The identifier of the new parent department or <c>null</c> to promote the department to root level.
    /// </summary>
    public Guid? NewParentDepartmentId { get; init; }
}
