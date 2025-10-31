using System;

namespace HR.Application.Common;

/// <summary>
///     Utility helpers for composing materialized paths used to model department hierarchies.
/// </summary>
internal static class DepartmentHierarchyPath
{
    public static string RootPrefix(Guid organizationId)
    {
        return $"/org/{organizationId}/dept";
    }

    public static string Combine(string prefix, Guid departmentId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(prefix);
        return $"{prefix.TrimEnd('/')}/{departmentId}";
    }

    public static string Build(Guid organizationId, Guid departmentId, string? parentPath)
    {
        var prefix = string.IsNullOrWhiteSpace(parentPath)
            ? RootPrefix(organizationId)
            : parentPath!;

        return Combine(prefix, departmentId);
    }
}
