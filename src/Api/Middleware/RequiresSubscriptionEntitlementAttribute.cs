using HR.Application.Configuration;

namespace HR.Api.Middleware;

/// <summary>
///     Marks endpoints that require a specific subscription entitlement.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public sealed class RequiresSubscriptionEntitlementAttribute(HrFeature feature) : Attribute
{
    public HrFeature Feature { get; } = feature;
}
