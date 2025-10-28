namespace HR.Application.DTOs;

/// <summary>
///     Represents usage consumption for a subscription feature entitlement.
/// </summary>
public sealed record UsageSummaryDto(
    Guid SubscriptionId,
    string Feature,
    int ConsumedUnits,
    int? UsageLimit);
