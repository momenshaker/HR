using Microsoft.AspNetCore.Mvc.Filters;

namespace HR.Api.Filters;

/// <summary>
///     Associates a resource/entity name with a controller for audit logging.
/// </summary>
public sealed class AuditResourceAttribute(string resourceName) : Attribute, IFilterMetadata
{
    public string ResourceName { get; } = resourceName;
}
