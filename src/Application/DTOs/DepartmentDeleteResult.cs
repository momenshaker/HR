namespace HR.Application.DTOs;

/// <summary>
///     Represents the outcome of a department delete operation.
/// </summary>
public sealed record DepartmentDeleteResult(bool Succeeded, bool NotFound, bool BlockedByChildren, int DeletedCount)
{
    public static DepartmentDeleteResult Success(int deletedCount) => new(true, false, false, deletedCount);

    public static DepartmentDeleteResult Missing() => new(false, true, false, 0);

    public static DepartmentDeleteResult Blocked() => new(false, false, true, 0);
}
